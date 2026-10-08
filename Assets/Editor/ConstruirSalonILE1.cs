// =====================================================================
//  ConstruirSalonILE1.cs
//  Actividad N°1 - Aspectos básicos en 3D
//
//  Este script añade un menú nuevo en Unity:  ILE-1 > Construir salon completo
//  Al hacer clic construye el salón ILE-1 usando SOLO objetos primitivos
//  (cubos, esferas, cilindros), crea los PREFABS y guarda la escena.
//
//  IMPORTANTE: este archivo debe estar dentro de una carpeta llamada "Editor"
//  (Assets/Editor/). Unity lo detecta solo, no hay que arrastrarlo a ningún lado.
//
//  Medidas: 1 unidad de Unity = 1 metro.
//  El suelo está en y = 0. El salón mide 10 m (X) x 7 m (Z) x 3 m (alto).
// =====================================================================
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public static class ConstruirSalonILE1
{
    // Carpeta donde se guardan escena, prefabs y materiales
    const string Raiz = "Assets/ILE1_Salon";

    // Diccionario con los materiales (colores) que vamos creando
    static Dictionary<string, Material> materiales = new Dictionary<string, Material>();

    // -----------------------------------------------------------------
    //  PUNTO DE ENTRADA: el menú de Unity
    // -----------------------------------------------------------------
    [MenuItem("ILE-1/Construir salon completo")]
    public static void Construir()
    {
        // Si hay una escena con cambios sin guardar, Unity pregunta primero
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        // Escena nueva (trae Main Camera y Directional Light)
        var escena = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        CrearCarpetas();
        CrearMateriales();

        // 1) Primero los PREFABS (los "moldes" reutilizables)
        GameObject prefabMesa = CrearPrefabMesa();
        GameObject prefabPuesto = CrearPrefabPuestoPC();
        GameObject prefabSilla = CrearPrefabSilla();
        GameObject prefabLed = CrearPrefabTiraLED();

        // 2) Luego el salón, que usa esos prefabs muchas veces
        var salon = new GameObject("Salon_ILE1");
        Transform estructura = Grupo("Estructura", salon.transform);
        Transform mobiliario = Grupo("Mobiliario", salon.transform);
        Transform equipos = Grupo("Equipos", salon.transform);
        Transform iluminacion = Grupo("Iluminacion", salon.transform);

        ConstruirEstructura(estructura);
        ConstruirEquipos(equipos);
        ColocarMobiliario(mobiliario, prefabMesa, prefabPuesto, prefabSilla);
        ColocarIluminacion(iluminacion, prefabLed);
        ConfigurarCamara();

        // 3) Guardar la escena
        EditorSceneManager.SaveScene(escena, Raiz + "/Escenas/Salon_ILE1.unity");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Listo",
            "El salon ILE-1 fue construido.\n\nEscena: " + Raiz + "/Escenas/Salon_ILE1.unity\n" +
            "Prefabs: " + Raiz + "/Prefabs", "OK");
    }

    // -----------------------------------------------------------------
    //  PREFABS
    // -----------------------------------------------------------------

    // Mesa larga y blanca (la de las fotos). Pivote en el suelo, centro de la mesa.
    static GameObject CrearPrefabMesa()
    {
        var raiz = new GameObject("MesaLarga");
        P(PrimitiveType.Cube, "Tablero", raiz.transform, new Vector3(0, 0.725f, 0), new Vector3(1.4f, 0.05f, 4.4f), M("TableroBlanco"));
        P(PrimitiveType.Cube, "Faldon", raiz.transform, new Vector3(0, 0.5f, 0), new Vector3(0.03f, 0.5f, 3.6f), M("MetalGris"));
        P(PrimitiveType.Cube, "PanelExtremo1", raiz.transform, new Vector3(0, 0.36f, 2.1f), new Vector3(1.2f, 0.72f, 0.05f), M("MetalGris"));
        P(PrimitiveType.Cube, "PanelExtremo2", raiz.transform, new Vector3(0, 0.36f, -2.1f), new Vector3(1.2f, 0.72f, 0.05f), M("MetalGris"));
        return GuardarPrefab(raiz, "MesaLarga");
    }

    // Monitor + teclado + mouse + mousepad. El pivote está sobre la mesa (y = 0 es la superficie).
    // El estudiante se sienta del lado -Z y la pantalla mira hacia él.
    static GameObject CrearPrefabPuestoPC()
    {
        var raiz = new GameObject("PuestoPC");
        Transform t = raiz.transform;

        // Monitor
        P(PrimitiveType.Cylinder, "BaseMonitor", t, new Vector3(0, 0.01f, 0.115f), new Vector3(0.22f, 0.01f, 0.22f), M("Negro"));
        P(PrimitiveType.Cube, "CuelloMonitor", t, new Vector3(0, 0.13f, 0.115f), new Vector3(0.05f, 0.22f, 0.04f), M("Negro"));
        P(PrimitiveType.Cube, "MarcoMonitor", t, new Vector3(0, 0.38f, 0.1f), new Vector3(0.55f, 0.32f, 0.03f), M("Negro"));
        P(PrimitiveType.Cube, "PantallaMonitor", t, new Vector3(0, 0.38f, 0.0835f), new Vector3(0.5f, 0.28f, 0.006f), M("PantallaEncendida"));

        // Teclado, mousepad y mouse
        P(PrimitiveType.Cube, "Teclado", t, new Vector3(0, 0.01f, -0.2f), new Vector3(0.42f, 0.02f, 0.14f), M("GrisOscuro"));
        P(PrimitiveType.Cube, "MousePad", t, new Vector3(0.35f, 0.002f, -0.2f), new Vector3(0.22f, 0.004f, 0.2f), M("AzulOscuro"));
        P(PrimitiveType.Sphere, "Mouse", t, new Vector3(0.35f, 0.019f, -0.2f), new Vector3(0.06f, 0.03f, 0.1f), M("GrisOscuro"));
        return GuardarPrefab(raiz, "PuestoPC");
    }

    // Silla con ruedas. Pivote en el suelo. El frente de la silla mira hacia +Z.
    static GameObject CrearPrefabSilla()
    {
        var raiz = new GameObject("Silla");
        Transform t = raiz.transform;

        // Base de 2 barras cruzadas + 4 ruedas
        P(PrimitiveType.Cube, "BaseBarraA", t, new Vector3(0, 0.02f, 0), new Vector3(0.55f, 0.04f, 0.06f), M("SillaGris"));
        P(PrimitiveType.Cube, "BaseBarraB", t, new Vector3(0, 0.02f, 0), new Vector3(0.06f, 0.04f, 0.55f), M("SillaGris"));
        P(PrimitiveType.Sphere, "Rueda1", t, new Vector3(0.25f, 0.025f, 0), new Vector3(0.05f, 0.05f, 0.05f), M("Negro"));
        P(PrimitiveType.Sphere, "Rueda2", t, new Vector3(-0.25f, 0.025f, 0), new Vector3(0.05f, 0.05f, 0.05f), M("Negro"));
        P(PrimitiveType.Sphere, "Rueda3", t, new Vector3(0, 0.025f, 0.25f), new Vector3(0.05f, 0.05f, 0.05f), M("Negro"));
        P(PrimitiveType.Sphere, "Rueda4", t, new Vector3(0, 0.025f, -0.25f), new Vector3(0.05f, 0.05f, 0.05f), M("Negro"));

        // Pistón, asiento y respaldo
        P(PrimitiveType.Cylinder, "Piston", t, new Vector3(0, 0.23f, 0), new Vector3(0.06f, 0.22f, 0.06f), M("MetalGris"));
        P(PrimitiveType.Cube, "Asiento", t, new Vector3(0, 0.46f, 0), new Vector3(0.45f, 0.06f, 0.45f), M("SillaGris"));
        P(PrimitiveType.Cube, "Respaldo", t, new Vector3(0, 0.75f, -0.2f), new Vector3(0.45f, 0.5f, 0.05f), M("SillaGris"));
        return GuardarPrefab(raiz, "Silla");
    }

    // Tira de luz LED del techo
    static GameObject CrearPrefabTiraLED()
    {
        var raiz = new GameObject("TiraLED");
        P(PrimitiveType.Cube, "Tira", raiz.transform, Vector3.zero, new Vector3(0.15f, 0.03f, 5f), M("LuzLED"));
        return GuardarPrefab(raiz, "TiraLED");
    }

    // -----------------------------------------------------------------
    //  ESTRUCTURA: piso, paredes, vidrio, puerta, techo
    // -----------------------------------------------------------------
    static void ConstruirEstructura(Transform e)
    {
        // Piso de madera + alfombra azul
        P(PrimitiveType.Cube, "Piso", e, new Vector3(0, -0.05f, 0), new Vector3(10.2f, 0.1f, 7.2f), M("PisoMadera"));
        P(PrimitiveType.Cube, "AlfombraAzul", e, new Vector3(0, 0.002f, 0.4f), new Vector3(6.6f, 0.004f, 5.8f), M("AlfombraAzul"));

        // Pared del fondo (donde está el TV)
        P(PrimitiveType.Cube, "ParedFondo", e, new Vector3(0, 1.5f, 3.55f), new Vector3(10.2f, 3f, 0.1f), M("ParedBlanca"));

        // Pared del frente con hueco para la puerta (puerta en x = 3.5 .. 4.5)
        P(PrimitiveType.Cube, "ParedFrente_Izq", e, new Vector3(-0.8f, 1.5f, -3.55f), new Vector3(8.6f, 3f, 0.1f), M("ParedBlanca"));
        P(PrimitiveType.Cube, "ParedFrente_Der", e, new Vector3(4.8f, 1.5f, -3.55f), new Vector3(0.6f, 3f, 0.1f), M("ParedBlanca"));
        P(PrimitiveType.Cube, "ParedFrente_SobrePuerta", e, new Vector3(4f, 2.6f, -3.55f), new Vector3(1f, 0.8f, 0.1f), M("ParedBlanca"));

        // Pared derecha (sólida) e izquierda (de vidrio)
        P(PrimitiveType.Cube, "ParedDerecha", e, new Vector3(5.05f, 1.5f, 0), new Vector3(0.1f, 3f, 7f), M("ParedBlanca"));
        P(PrimitiveType.Cube, "ParedVidrio", e, new Vector3(-5.05f, 1.5f, 0), new Vector3(0.1f, 3f, 7f), M("Vidrio"));
        P(PrimitiveType.Cube, "MarcoVidrio1", e, new Vector3(-5.05f, 1.5f, -1.17f), new Vector3(0.14f, 3f, 0.1f), M("MetalGris"));
        P(PrimitiveType.Cube, "MarcoVidrio2", e, new Vector3(-5.05f, 1.5f, 1.17f), new Vector3(0.14f, 3f, 0.1f), M("MetalGris"));

        // Techo (puedes desactivarlo en el Inspector para ver el salón desde arriba)
        P(PrimitiveType.Cube, "Techo", e, new Vector3(0, 3.05f, 0), new Vector3(10.2f, 0.1f, 7.2f), M("ParedBlanca"));

        // Puerta plateada con cartel azul "ILE-1" (solo primitivos, sin texto)
        P(PrimitiveType.Cube, "Puerta", e, new Vector3(4f, 1.1f, -3.525f), new Vector3(1f, 2.2f, 0.05f), M("Plata"));
        P(PrimitiveType.Cube, "ManijaPuerta", e, new Vector3(3.62f, 1.0f, -3.49f), new Vector3(0.03f, 0.15f, 0.06f), M("MetalGris"));
        P(PrimitiveType.Cube, "PanelCartel", e, new Vector3(3.1f, 1.1f, -3.485f), new Vector3(0.3f, 2f, 0.03f), M("Plata"));
        P(PrimitiveType.Cube, "CartelAzul_ILE1", e, new Vector3(3.1f, 1.7f, -3.47f), new Vector3(0.06f, 0.7f, 0.02f), M("AzulCartel"));
    }

    // -----------------------------------------------------------------
    //  EQUIPOS: TV, pantalla de proyector, pizarra, papelera
    // -----------------------------------------------------------------
    static void ConstruirEquipos(Transform e)
    {
        // TV en la pared del fondo (la pared interior está en z = 3.5)
        P(PrimitiveType.Cube, "TV", e, new Vector3(0, 1.9f, 3.47f), new Vector3(1.8f, 1.0f, 0.06f), M("Negro"));
        P(PrimitiveType.Cube, "TV_Pantalla", e, new Vector3(0, 1.9f, 3.4385f), new Vector3(1.7f, 0.9f, 0.006f), M("PantallaApagada"));

        // Pantalla de proyector en la pared derecha (pared interior en x = 5.0)
        P(PrimitiveType.Cube, "Proyector_Marco", e, new Vector3(4.985f, 1.9f, 0), new Vector3(0.03f, 1.7f, 2.5f), M("Negro"));
        P(PrimitiveType.Cube, "Proyector_Tela", e, new Vector3(4.968f, 1.9f, 0), new Vector3(0.01f, 1.5f, 2.3f), M("ProyectorBlanco"));
        P(PrimitiveType.Cylinder, "Proyector_Rollo", e, new Vector3(4.93f, 2.78f, 0), new Vector3(0.1f, 1.25f, 0.1f), M("ProyectorBlanco"), new Vector3(90, 0, 0));

        // Pizarra blanca al lado
        P(PrimitiveType.Cube, "Pizarra", e, new Vector3(4.985f, 1.5f, 2.3f), new Vector3(0.03f, 1.0f, 1.4f), M("TableroBlanco"));

        // Papelera y purificador en la esquina del vidrio
        P(PrimitiveType.Cylinder, "Papelera", e, new Vector3(-4.6f, 0.2f, 3.0f), new Vector3(0.3f, 0.2f, 0.3f), M("GrisOscuro"));
        P(PrimitiveType.Cube, "Purificador", e, new Vector3(-4.7f, 0.225f, 2.4f), new Vector3(0.25f, 0.45f, 0.2f), M("Negro"));
    }

    // -----------------------------------------------------------------
    //  MOBILIARIO: usa los PREFABS muchas veces (esto es lo que pide la rúbrica)
    // -----------------------------------------------------------------
    static void ColocarMobiliario(Transform padre, GameObject mesa, GameObject puesto, GameObject silla)
    {
        float[] centrosMesaX = { -2f, 2f };            // dos mesas largas
        float centroMesaZ = 0.4f;
        float[] desplazamientosZ = { -1.35f, -0.45f, 0.45f, 1.35f };   // 4 puestos por lado
        int numMesa = 1;

        foreach (float cx in centrosMesaX)
        {
            Colocar(mesa, padre, "Mesa_" + numMesa, new Vector3(cx, 0, centroMesaZ), 0);

            foreach (int lado in new int[] { -1, 1 })   // -1 = lado oeste, +1 = lado este
            {
                float giro = lado < 0 ? 90f : -90f;     // para que miren hacia la mesa
                int n = 1;
                foreach (float dz in desplazamientosZ)
                {
                    float z = centroMesaZ + dz;
                    string sufijo = "_M" + numMesa + (lado < 0 ? "_Oeste_" : "_Este_") + n;
                    Colocar(puesto, padre, "PuestoPC" + sufijo, new Vector3(cx + lado * 0.3f, 0.75f, z), giro);
                    Colocar(silla, padre, "Silla" + sufijo, new Vector3(cx + lado * 0.75f, 0, z), giro);
                    n++;
                }
            }
            numMesa++;
        }
    }

    static void ColocarIluminacion(Transform padre, GameObject tiraLed)
    {
        // Tiras LED pegadas al techo (el techo está en y = 3.0 por debajo)
        float[] xs = { -3f, 0f, 3f };
        for (int i = 0; i < xs.Length; i++)
            Colocar(tiraLed, padre, "TiraLED_" + (i + 1), new Vector3(xs[i], 2.985f, 0.4f), 0);

        // Luces reales para que el salón se vea iluminado por dentro
        Vector3[] posiciones = {
            new Vector3(-2, 2.6f, -1.5f), new Vector3(2, 2.6f, -1.5f),
            new Vector3(-2, 2.6f, 2f),    new Vector3(2, 2.6f, 2f)
        };
        for (int i = 0; i < posiciones.Length; i++)
        {
            var go = new GameObject("LuzPuntual_" + (i + 1));
            go.transform.SetParent(padre, false);
            go.transform.localPosition = posiciones[i];
            var luz = go.AddComponent<Light>();
            luz.type = LightType.Point;
            luz.range = 7f;
            luz.intensity = 1.5f;   // si se ve muy oscuro o muy claro, cambia este número en el Inspector
            luz.color = new Color(1f, 0.97f, 0.92f);
            luz.shadows = LightShadows.None;
        }

        // La luz direccional del sol quedaría tapada por el techo; la dejamos suave y sin sombras
        var sol = GameObject.Find("Directional Light");
        if (sol != null)
        {
            var l = sol.GetComponent<Light>();
            if (l != null) { l.intensity = 0.4f; l.shadows = LightShadows.None; }
        }
    }

    static void ConfigurarCamara()
    {
        var cam = Camera.main;
        if (cam == null) return;
        cam.transform.position = new Vector3(4.2f, 1.7f, -3.0f);
        cam.transform.LookAt(new Vector3(-1f, 1.0f, 2f));
    }

    // -----------------------------------------------------------------
    //  HERRAMIENTAS (funciones de ayuda)
    // -----------------------------------------------------------------

    // Crea un primitivo, lo nombra, lo mete en un padre y le pone posición/tamaño/color
    static GameObject P(PrimitiveType tipo, string nombre, Transform padre, Vector3 pos, Vector3 escala, Material mat, Vector3 rot = default(Vector3))
    {
        var go = GameObject.CreatePrimitive(tipo);
        go.name = nombre;
        go.transform.SetParent(padre, false);
        go.transform.localPosition = pos;
        go.transform.localEulerAngles = rot;
        go.transform.localScale = escala;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        return go;
    }

    // Pone una copia (instancia) de un prefab en la escena
    static void Colocar(GameObject prefab, Transform padre, string nombre, Vector3 pos, float giroY)
    {
        var inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab, padre);
        inst.name = nombre;
        inst.transform.localPosition = pos;
        inst.transform.localEulerAngles = new Vector3(0, giroY, 0);
    }

    static Transform Grupo(string nombre, Transform padre)
    {
        var go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        return go.transform;
    }

    // Guarda un objeto de la escena como archivo .prefab y borra el temporal
    static GameObject GuardarPrefab(GameObject temporal, string nombre)
    {
        string ruta = Raiz + "/Prefabs/" + nombre + ".prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(temporal, ruta);
        Object.DestroyImmediate(temporal);
        return prefab;
    }

    static void CrearCarpetas()
    {
        if (!AssetDatabase.IsValidFolder(Raiz)) AssetDatabase.CreateFolder("Assets", "ILE1_Salon");
        foreach (string sub in new string[] { "Prefabs", "Materiales", "Escenas" })
            if (!AssetDatabase.IsValidFolder(Raiz + "/" + sub)) AssetDatabase.CreateFolder(Raiz, sub);
    }

    // -----------------------------------------------------------------
    //  MATERIALES (colores). Funciona con render pipeline normal (Built-in) y con URP.
    // -----------------------------------------------------------------
    static Material M(string nombre) { return materiales[nombre]; }

    static void CrearMateriales()
    {
        materiales.Clear();
        Crear("ParedBlanca", new Color(0.93f, 0.93f, 0.92f));
        Crear("PisoMadera", new Color(0.58f, 0.48f, 0.40f));
        Crear("AlfombraAzul", new Color(0.10f, 0.20f, 0.55f));
        Crear("TableroBlanco", new Color(0.96f, 0.96f, 0.96f));
        Crear("MetalGris", new Color(0.62f, 0.64f, 0.66f));
        Crear("Plata", new Color(0.78f, 0.79f, 0.80f));
        Crear("Negro", new Color(0.07f, 0.07f, 0.08f));
        Crear("GrisOscuro", new Color(0.18f, 0.18f, 0.20f));
        Crear("SillaGris", new Color(0.25f, 0.27f, 0.30f));
        Crear("AzulOscuro", new Color(0.08f, 0.12f, 0.35f));
        Crear("AzulCartel", new Color(0.15f, 0.20f, 0.70f));
        Crear("PantallaApagada", new Color(0.04f, 0.04f, 0.06f));
        Crear("ProyectorBlanco", new Color(0.98f, 0.98f, 0.98f));
        Crear("PantallaEncendida", new Color(0.20f, 0.45f, 0.90f), false, true);
        Crear("LuzLED", new Color(1f, 1f, 1f), false, true);
        Crear("Vidrio", new Color(0.65f, 0.80f, 0.90f, 0.25f), true, false);
    }

    static void Crear(string nombre, Color color, bool transparente = false, bool emisivo = false)
    {
        // Si el proyecto usa URP el shader se llama distinto
        Shader sh = null;
        if (GraphicsSettings.currentRenderPipeline != null)
            sh = Shader.Find("Universal Render Pipeline/Lit");
        if (sh == null) sh = Shader.Find("Standard");

        var mat = new Material(sh);
        mat.name = nombre;
        mat.color = color;

        if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0.2f);
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0.2f);

        if (emisivo)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", color * 1.5f);
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }

        if (transparente)
        {
            if (mat.HasProperty("_Surface"))
            {
                // URP
                mat.SetFloat("_Surface", 1f);
                mat.SetFloat("_Blend", 0f);
                mat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
                mat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                mat.SetFloat("_ZWrite", 0f);
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.SetShaderPassEnabled("ShadowCaster", false);
            }
            else
            {
                // Built-in (Standard)
                mat.SetFloat("_Mode", 3f);
                mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            }
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.renderQueue = 3000;
        }

        // Guardar el material como archivo para que los prefabs lo puedan usar
        AssetDatabase.CreateAsset(mat, Raiz + "/Materiales/" + nombre + ".mat");
        materiales[nombre] = mat;
    }
}
