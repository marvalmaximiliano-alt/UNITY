// ConstruirCinematica.cs
// Añade dos opciones al menu "Arcade":
//   - Construir cinematica: crea la escena de la cinematica lista para darle a Play.
//   - Instalar Unity Recorder: instala el paquete que sirve para grabar el video del Game View con sonido.
// Debe estar dentro de una carpeta llamada "Editor".
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ConstruirCinematica
{
    const string Raiz = "Assets/StarRush";
    const string RutaEscena = "Assets/StarRush/Escenas/Cinematica.unity";

    [MenuItem("Arcade/Construir cinematica")]
    public static void Construir()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        // Escena nueva (trae Main Camera y Directional Light)
        var escena = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        if (!AssetDatabase.IsValidFolder(Raiz)) AssetDatabase.CreateFolder("Assets", "StarRush");
        if (!AssetDatabase.IsValidFolder(Raiz + "/Escenas")) AssetDatabase.CreateFolder(Raiz, "Escenas");

        // Camara: la metemos dentro de un "rig" para poder moverla y sacudirla
        Camera cam = Camera.main;
        if (cam == null)
        {
            var camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            cam = camGO.AddComponent<Camera>();
            camGO.AddComponent<AudioListener>();
        }

        var rig = new GameObject("RigCamara");
        var director = rig.AddComponent<DirectorCamara>();
        cam.transform.SetParent(rig.transform, false);
        cam.transform.localPosition = Vector3.zero;
        cam.transform.localRotation = Quaternion.identity;
        director.cam = cam;

        // Luz del "sol"
        Light sol = null;
        GameObject solGO = GameObject.Find("Directional Light");
        if (solGO != null) sol = solGO.GetComponent<Light>();
        if (sol == null)
        {
            solGO = new GameObject("Directional Light");
            sol = solGO.AddComponent<Light>();
            sol.type = LightType.Directional;
        }

        // El objeto que contiene el guion
        var go = new GameObject("Cinematica");
        var cine = go.AddComponent<Cinematica>();
        cine.director = director;
        cine.sol = sol;
        cine.clipMusica = Clip("cine_musica");
        cine.clipAlarma = Clip("cine_alarma");
        cine.clipWhoosh = Clip("cine_whoosh");
        cine.clipLaser = Clip("cine_laser");
        cine.clipRugido = Clip("cine_rugido");
        cine.clipMotor = Clip("cine_motor");
        cine.clipExplosion = Clip("explosion");
        cine.clipDisparo = Clip("disparo");
        cine.clipEstrella = Clip("estrella");

        EditorSceneManager.SaveScene(escena, RutaEscena);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Listo",
            "La cinematica esta construida.\n\n" +
            "1) Pulsa PLAY (triangulo de arriba) para verla.\n" +
            "2) Para poner los nombres del equipo: selecciona el objeto 'Cinematica' en la Hierarchy y edita 'Creditos' en el Inspector.\n" +
            "3) Para grabar el video usa Unity Recorder (menu Arcade).",
            "OK");
    }

    [MenuItem("Arcade/Instalar Unity Recorder (para grabar el video)")]
    public static void InstalarRecorder()
    {
        Client.Add("com.unity.recorder");
        EditorUtility.DisplayDialog("Instalando",
            "Unity esta instalando el paquete Recorder.\n\n" +
            "Cuando termine (ruedita abajo a la derecha), abre:\nWindow > General > Recorder > Recorder Window",
            "OK");
    }

    static AudioClip Clip(string nombre)
    {
        return AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/" + nombre + ".wav");
    }
}
