// ConstruirJuego.cs
// Añade el menu  Arcade > Construir juego completo
// Al hacer clic crea TODO lo necesario para jugar a "Star Rush":
//   - materiales (colores), prefabs (nave, bala, asteroide, estrella)
//   - la escena con camara, fondo, GameManager, audio y toda la interfaz (menu, HUD, pausa, game over)
// Debe estar dentro de una carpeta llamada "Editor".
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public static class ConstruirJuego
{
    const string Raiz = "Assets/StarRush";
    static Dictionary<string, Material> materiales = new Dictionary<string, Material>();

    [MenuItem("Arcade/Construir juego completo")]
    public static void Construir()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        // Escena nueva con camara y luz
        var escena = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        CrearCarpetas();
        CrearMateriales();

        // ---- 1) Prefabs ----
        GameObject prefabBala = CrearPrefabBala();
        GameObject prefabJugador = CrearPrefabJugador(prefabBala);
        GameObject prefabAsteroide = CrearPrefabAsteroide();
        GameObject prefabEstrella = CrearPrefabEstrella();

        // ---- 2) Camara ----
        ConfigurarCamara();

        // ---- 3) Objetos de la escena ----
        var jugadorGO = (GameObject)PrefabUtility.InstantiatePrefab(prefabJugador);
        jugadorGO.name = "Jugador";

        var fondoGO = new GameObject("FondoEstrellas");
        var fondo = fondoGO.AddComponent<FondoEstrellas>();
        fondo.material = M("EstrellaFondo");

        var audioGO = new GameObject("GestorAudio");
        var audio = audioGO.AddComponent<GestorAudio>();
        audio.clipMusica = Clip("musica");
        audio.clipDisparo = Clip("disparo");
        audio.clipExplosion = Clip("explosion");
        audio.clipEstrella = Clip("estrella");
        audio.clipGolpe = Clip("golpe");
        audio.clipNivel = Clip("nivel");
        audio.clipFinJuego = Clip("gameover");

        var gmGO = new GameObject("GameManager");
        var gm = gmGO.AddComponent<GameManager>();
        gm.prefabAsteroide = prefabAsteroide;
        gm.prefabEstrella = prefabEstrella;
        gm.jugador = jugadorGO.GetComponent<Jugador>();

        // ---- 4) Interfaz (Canvas) ----
        ConstruirInterfaz(gm);

        // ---- 5) Guardar escena y añadirla al Build ----
        string rutaEscena = Raiz + "/Escenas/StarRush.unity";
        EditorSceneManager.SaveScene(escena, rutaEscena);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(rutaEscena, true) };
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Listo",
            "Star Rush esta construido.\n\nPulsa el boton PLAY (triangulo de arriba) para jugar.\n\n" +
            "Escena: " + rutaEscena, "OK");
    }

    // =====================================================================
    //  PREFABS
    // =====================================================================
    static GameObject CrearPrefabBala()
    {
        var raiz = new GameObject("Bala");
        P(PrimitiveType.Capsule, "Cuerpo", raiz.transform, Vector3.zero, new Vector3(0.18f, 0.35f, 0.18f), M("Bala"));

        var col = raiz.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(0.25f, 0.7f, 0.5f);

        var rb = raiz.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        raiz.AddComponent<Bala>();
        return GuardarPrefab(raiz, "Bala");
    }

    static GameObject CrearPrefabJugador(GameObject prefabBala)
    {
        var raiz = new GameObject("Jugador");
        Transform t = raiz.transform;

        // La nave esta hecha solo con primitivos
        P(PrimitiveType.Cube, "Cuerpo", t, Vector3.zero, new Vector3(0.5f, 1.1f, 0.3f), M("Cian"));
        P(PrimitiveType.Cube, "AlaIzq", t, new Vector3(-0.5f, -0.25f, 0f), new Vector3(0.6f, 0.3f, 0.2f), M("Cian"), new Vector3(0f, 0f, 25f));
        P(PrimitiveType.Cube, "AlaDer", t, new Vector3(0.5f, -0.25f, 0f), new Vector3(0.6f, 0.3f, 0.2f), M("Cian"), new Vector3(0f, 0f, -25f));
        P(PrimitiveType.Sphere, "Cabina", t, new Vector3(0f, 0.2f, -0.2f), new Vector3(0.35f, 0.45f, 0.3f), M("Blanco"));
        P(PrimitiveType.Cube, "Llama", t, new Vector3(0f, -0.75f, 0f), new Vector3(0.22f, 0.35f, 0.1f), M("Naranja"));

        var col = raiz.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(1.3f, 1.2f, 0.6f);

        var rb = raiz.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var jugador = raiz.AddComponent<Jugador>();
        jugador.prefabBala = prefabBala;
        return GuardarPrefab(raiz, "Jugador");
    }

    static GameObject CrearPrefabAsteroide()
    {
        var raiz = new GameObject("Asteroide");
        Transform t = raiz.transform;

        P(PrimitiveType.Sphere, "Roca", t, Vector3.zero, Vector3.one, M("Asteroide"));
        P(PrimitiveType.Sphere, "Crater1", t, new Vector3(0.2f, 0.2f, -0.42f), new Vector3(0.28f, 0.28f, 0.1f), M("Crater"));
        P(PrimitiveType.Sphere, "Crater2", t, new Vector3(-0.22f, -0.1f, -0.42f), new Vector3(0.2f, 0.2f, 0.1f), M("Crater"));
        P(PrimitiveType.Sphere, "Crater3", t, new Vector3(0.05f, -0.28f, -0.44f), new Vector3(0.15f, 0.15f, 0.1f), M("Crater"));

        var col = raiz.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 0.5f;

        var rb = raiz.AddComponent<Rigidbody>();
        rb.isKinematic = false;     // dinamico sin gravedad: asi detecta bien a la nave y a las balas
        rb.useGravity = false;

        raiz.AddComponent<Asteroide>();
        return GuardarPrefab(raiz, "Asteroide");
    }

    static GameObject CrearPrefabEstrella()
    {
        var raiz = new GameObject("Estrella");
        Transform t = raiz.transform;

        // Estrella de 8 puntas = dos cuadrados girados 45 grados
        P(PrimitiveType.Cube, "Cuadrado1", t, Vector3.zero, new Vector3(0.6f, 0.6f, 0.2f), M("Amarillo"));
        P(PrimitiveType.Cube, "Cuadrado2", t, Vector3.zero, new Vector3(0.6f, 0.6f, 0.2f), M("Amarillo"), new Vector3(0f, 0f, 45f));

        var col = raiz.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 0.5f;

        var rb = raiz.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = false;

        raiz.AddComponent<Estrella>();
        return GuardarPrefab(raiz, "Estrella");
    }

    // =====================================================================
    //  CAMARA
    // =====================================================================
    static void ConfigurarCamara()
    {
        Camera cam = Camera.main;
        if (cam == null) return;
        cam.orthographic = true;
        cam.orthographicSize = 6f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.03f, 0.03f, 0.10f);
        cam.transform.position = new Vector3(0f, 0f, -10f);
        cam.transform.rotation = Quaternion.identity;
    }

    // =====================================================================
    //  INTERFAZ
    // =====================================================================
    static void ConstruirInterfaz(GameManager gm)
    {
        // Canvas
        var canvasGO = new GameObject("Canvas", typeof(RectTransform));
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var escalador = canvasGO.AddComponent<CanvasScaler>();
        escalador.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escalador.referenceResolution = new Vector2(1920f, 1080f);
        escalador.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // EventSystem (necesario para que funcionen los botones con el mouse)
        var esGO = new GameObject("EventSystem");
        esGO.AddComponent<EventSystem>();
        System.Type tipoNuevo = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
        if (tipoNuevo != null)
        {
            Component modulo = esGO.AddComponent(tipoNuevo);
            var metodo = tipoNuevo.GetMethod("AssignDefaultActions");
            if (metodo != null) metodo.Invoke(modulo, null);
        }
        else
        {
            esGO.AddComponent<StandaloneInputModule>();
        }

        Color cian = new Color(0.3f, 0.95f, 1f);
        Color blanco = Color.white;
        Color amarillo = new Color(1f, 0.9f, 0.2f);
        Color rojo = new Color(1f, 0.3f, 0.3f);
        Color fondoOscuro = new Color(0f, 0f, 0.05f, 0.65f);
        Color botonColor = new Color(0.1f, 0.45f, 0.7f, 1f);
        Vector2 centro = new Vector2(0.5f, 0.5f);

        // ---------- MENU PRINCIPAL ----------
        GameObject menu = CrearPanel("PanelMenu", canvasGO.transform, fondoOscuro, true);
        CrearTexto("Titulo", menu.transform, "STAR RUSH", 150, TextAnchor.MiddleCenter, cian, centro, new Vector2(0, 270), new Vector2(1400, 180));
        CrearTexto("Subtitulo", menu.transform, "Esquiva asteroides, dispara y recoge estrellas", 48, TextAnchor.MiddleCenter, blanco, centro, new Vector2(0, 140), new Vector2(1400, 80));
        gm.botonJugar = CrearBoton("BotonJugar", menu.transform, "JUGAR", new Vector2(0, 0), new Vector2(460, 100), botonColor);
        gm.botonSalir = CrearBoton("BotonSalir", menu.transform, "SALIR", new Vector2(0, -130), new Vector2(460, 100), botonColor);
        CrearTexto("Controles", menu.transform, "A / D o FLECHAS: mover     ESPACIO: disparar     P: pausa     ENTER: jugar", 36, TextAnchor.MiddleCenter, amarillo, centro, new Vector2(0, -290), new Vector2(1800, 60));
        CrearTexto("Objetivo", menu.transform, "Cada 5 estrellas ganas una vida extra. ¡Cada 300 puntos subes de nivel!", 32, TextAnchor.MiddleCenter, blanco, centro, new Vector2(0, -350), new Vector2(1800, 60));

        // ---------- HUD ----------
        GameObject hud = CrearPanel("PanelHUD", canvasGO.transform, Color.clear, false);
        gm.textoPuntos = CrearTexto("TextoPuntos", hud.transform, "PUNTOS: 0", 52, TextAnchor.UpperLeft, blanco, new Vector2(0, 1), new Vector2(40, -30), new Vector2(700, 70));
        gm.textoRecord = CrearTexto("TextoRecord", hud.transform, "RÉCORD: 0", 36, TextAnchor.UpperLeft, amarillo, new Vector2(0, 1), new Vector2(40, -100), new Vector2(700, 50));
        gm.textoNivel = CrearTexto("TextoNivel", hud.transform, "NIVEL 1", 52, TextAnchor.UpperCenter, cian, new Vector2(0.5f, 1), new Vector2(0, -30), new Vector2(500, 70));
        gm.textoVidas = CrearTexto("TextoVidas", hud.transform, "VIDAS: 3", 52, TextAnchor.UpperRight, rojo, new Vector2(1, 1), new Vector2(-40, -30), new Vector2(500, 70));
        gm.textoBanner = CrearTexto("TextoBanner", hud.transform, "NIVEL 2", 100, TextAnchor.MiddleCenter, amarillo, centro, new Vector2(0, 200), new Vector2(1200, 140));
        gm.textoBanner.gameObject.SetActive(false);

        // ---------- PAUSA ----------
        GameObject pausa = CrearPanel("PanelPausa", canvasGO.transform, fondoOscuro, true);
        CrearTexto("TituloPausa", pausa.transform, "PAUSA", 130, TextAnchor.MiddleCenter, cian, centro, new Vector2(0, 200), new Vector2(1000, 160));
        CrearTexto("AyudaPausa", pausa.transform, "Pulsa P para continuar", 44, TextAnchor.MiddleCenter, blanco, centro, new Vector2(0, 90), new Vector2(1000, 70));
        gm.botonReanudar = CrearBoton("BotonReanudar", pausa.transform, "CONTINUAR", new Vector2(0, -30), new Vector2(460, 100), botonColor);
        gm.botonMenuPausa = CrearBoton("BotonMenuPausa", pausa.transform, "MENÚ PRINCIPAL", new Vector2(0, -160), new Vector2(460, 100), botonColor);

        // ---------- GAME OVER ----------
        GameObject over = CrearPanel("PanelGameOver", canvasGO.transform, new Color(0.1f, 0f, 0f, 0.75f), true);
        CrearTexto("TituloGameOver", over.transform, "GAME OVER", 140, TextAnchor.MiddleCenter, rojo, centro, new Vector2(0, 230), new Vector2(1400, 180));
        gm.textoFinal = CrearTexto("TextoFinal", over.transform, "PUNTOS: 0\nRÉCORD: 0", 60, TextAnchor.MiddleCenter, blanco, centro, new Vector2(0, 60), new Vector2(1000, 180));
        gm.botonReintentar = CrearBoton("BotonReintentar", over.transform, "REINTENTAR", new Vector2(0, -120), new Vector2(460, 100), botonColor);
        gm.botonMenuFin = CrearBoton("BotonMenuFin", over.transform, "MENÚ PRINCIPAL", new Vector2(0, -250), new Vector2(460, 100), botonColor);
        CrearTexto("AyudaGameOver", over.transform, "o pulsa ESPACIO para reintentar", 36, TextAnchor.MiddleCenter, amarillo, centro, new Vector2(0, -350), new Vector2(1200, 60));

        gm.panelMenu = menu;
        gm.panelHUD = hud;
        gm.panelPausa = pausa;
        gm.panelGameOver = over;
    }

    static GameObject CrearPanel(string nombre, Transform padre, Color color, bool conFondo)
    {
        var go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        if (conFondo)
        {
            var img = go.AddComponent<Image>();
            img.color = color;
        }
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return go;
    }

    static Text CrearTexto(string nombre, Transform padre, string contenido, int tamano, TextAnchor alineacion,
                           Color color, Vector2 ancla, Vector2 posicion, Vector2 tamanoCaja)
    {
        var go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        var t = go.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.text = contenido;
        t.fontSize = tamano;
        t.alignment = alineacion;
        t.color = color;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        var rt = t.rectTransform;
        rt.anchorMin = ancla;
        rt.anchorMax = ancla;
        rt.pivot = ancla;
        rt.anchoredPosition = posicion;
        rt.sizeDelta = tamanoCaja;
        return t;
    }

    static Button CrearBoton(string nombre, Transform padre, string texto, Vector2 posicion, Vector2 tamano, Color color)
    {
        var go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        var img = go.AddComponent<Image>();
        img.color = color;
        var boton = go.AddComponent<Button>();
        boton.targetGraphic = img;

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = posicion;
        rt.sizeDelta = tamano;

        Text t = CrearTexto("Texto", go.transform, texto, 44, TextAnchor.MiddleCenter, Color.white,
                            new Vector2(0.5f, 0.5f), Vector2.zero, tamano);
        return boton;
    }

    // =====================================================================
    //  HERRAMIENTAS
    // =====================================================================
    // Crea un primitivo sin collider (el collider lo ponemos solo en la raiz de cada prefab)
    static GameObject P(PrimitiveType tipo, string nombre, Transform padre, Vector3 pos, Vector3 escala, Material mat, Vector3 rot = default(Vector3))
    {
        var go = GameObject.CreatePrimitive(tipo);
        go.name = nombre;
        Object.DestroyImmediate(go.GetComponent<Collider>());
        go.transform.SetParent(padre, false);
        go.transform.localPosition = pos;
        go.transform.localEulerAngles = rot;
        go.transform.localScale = escala;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        return go;
    }

    static GameObject GuardarPrefab(GameObject temporal, string nombre)
    {
        string ruta = Raiz + "/Prefabs/" + nombre + ".prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(temporal, ruta);
        Object.DestroyImmediate(temporal);
        return prefab;
    }

    static AudioClip Clip(string nombre)
    {
        return AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/" + nombre + ".wav");
    }

    static void CrearCarpetas()
    {
        if (!AssetDatabase.IsValidFolder(Raiz)) AssetDatabase.CreateFolder("Assets", "StarRush");
        foreach (string sub in new string[] { "Prefabs", "Materiales", "Escenas" })
            if (!AssetDatabase.IsValidFolder(Raiz + "/" + sub)) AssetDatabase.CreateFolder(Raiz, sub);
    }

    // ---- Materiales: sin luces (Unlit) para un look neon, funciona en URP y en Built-in ----
    static Material M(string nombre) { return materiales[nombre]; }

    static void CrearMateriales()
    {
        materiales.Clear();
        Crear("Cian", new Color(0.2f, 0.9f, 1f));
        Crear("Blanco", new Color(0.95f, 0.98f, 1f));
        Crear("Naranja", new Color(1f, 0.5f, 0.1f));
        Crear("Amarillo", new Color(1f, 0.9f, 0.2f));
        Crear("Bala", new Color(1f, 1f, 0.45f));
        Crear("Asteroide", new Color(1f, 0.35f, 0.25f));
        Crear("Crater", new Color(0.55f, 0.12f, 0.1f));
        Crear("EstrellaFondo", new Color(0.8f, 0.85f, 1f));
    }

    static void Crear(string nombre, Color color)
    {
        Shader sh = null;
        if (GraphicsSettings.currentRenderPipeline != null)
            sh = Shader.Find("Universal Render Pipeline/Unlit");
        if (sh == null) sh = Shader.Find("Unlit/Color");
        if (sh == null) sh = Shader.Find("Sprites/Default");

        var mat = new Material(sh);
        mat.name = nombre;
        mat.color = color;
        AssetDatabase.CreateAsset(mat, Raiz + "/Materiales/" + nombre + ".mat");
        materiales[nombre] = mat;
    }
}
