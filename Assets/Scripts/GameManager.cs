// GameManager.cs
// El "cerebro" del juego: controla los estados (menu, jugando, pausa, game over),
// la puntuacion, las vidas, los niveles, la aparicion de asteroides/estrellas y los textos de la interfaz.
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum Estado { Menu, Jugando, Pausa, GameOver }

    public static GameManager Instancia;

    [Header("Prefabs")]
    public GameObject prefabAsteroide;
    public GameObject prefabEstrella;

    [Header("Jugador")]
    public Jugador jugador;

    [Header("Paneles de la interfaz")]
    public GameObject panelMenu;
    public GameObject panelHUD;
    public GameObject panelPausa;
    public GameObject panelGameOver;

    [Header("Textos")]
    public Text textoPuntos;
    public Text textoNivel;
    public Text textoVidas;
    public Text textoRecord;
    public Text textoFinal;
    public Text textoBanner;

    [Header("Botones")]
    public Button botonJugar;
    public Button botonSalir;
    public Button botonReanudar;
    public Button botonMenuPausa;
    public Button botonReintentar;
    public Button botonMenuFin;

    public Estado estado = Estado.Menu;

    // Propiedades de ayuda para los otros scripts
    public bool Jugando { get { return estado == Estado.Jugando; } }
    public Transform Contenedor { get { return contenedor; } }
    public float MitadAlto { get { return cam.orthographicSize; } }
    public float MitadAncho { get { return cam.orthographicSize * cam.aspect; } }

    Camera cam;
    Transform contenedor;       // aqui se guardan asteroides, balas, etc. (para limpiarlos facil)
    int puntos, vidas, nivel, estrellasRecogidas, record;
    float temporizadorSpawn, temporizadorBanner, inicioGameOver;

    const int VidasIniciales = 3;
    const int VidasMaximas = 5;
    const int PuntosPorNivel = 300;

    void Awake()
    {
        Instancia = this;
        cam = Camera.main;
        contenedor = new GameObject("Contenedor").transform;
        record = PlayerPrefs.GetInt("record", 0);

        // Conectar los botones con sus acciones
        botonJugar.onClick.AddListener(Jugar);
        botonSalir.onClick.AddListener(Salir);
        botonReanudar.onClick.AddListener(Reanudar);
        botonMenuPausa.onClick.AddListener(IrAlMenu);
        botonReintentar.onClick.AddListener(Jugar);
        botonMenuFin.onClick.AddListener(IrAlMenu);
    }

    void Start()
    {
        IrAlMenu();
    }

    void Update()
    {
        switch (estado)
        {
            case Estado.Menu:
                if (Entrada.ConfirmarPresionado()) Jugar();
                break;

            case Estado.Jugando:
                if (Entrada.PausaPresionada()) { Pausar(); break; }
                ActualizarSpawn();
                ActualizarBanner();
                break;

            case Estado.Pausa:
                if (Entrada.PausaPresionada()) Reanudar();
                break;

            case Estado.GameOver:
                // Pequeña espera para no reiniciar sin querer si sigues apretando espacio
                if (Time.unscaledTime - inicioGameOver > 1f && Entrada.ConfirmarPresionado()) Jugar();
                break;
        }
    }

    // ------------------------------------------------------------------
    //  Cambios de estado
    // ------------------------------------------------------------------
    public void IrAlMenu()
    {
        LimpiarContenedor();
        estado = Estado.Menu;
        Time.timeScale = 0f;
        jugador.gameObject.SetActive(false);
        MostrarPaneles(true, false, false, false);
        GestorAudio.IniciarMusica();
    }

    public void Jugar()
    {
        LimpiarContenedor();
        puntos = 0;
        vidas = VidasIniciales;
        nivel = 1;
        estrellasRecogidas = 0;
        temporizadorSpawn = 1f;
        temporizadorBanner = 0f;
        textoBanner.gameObject.SetActive(false);

        jugador.Reiniciar();
        estado = Estado.Jugando;
        Time.timeScale = 1f;
        MostrarPaneles(false, true, false, false);
        ActualizarHUD();
        GestorAudio.IniciarMusica();
    }

    void Pausar()
    {
        estado = Estado.Pausa;
        Time.timeScale = 0f;
        MostrarPaneles(false, true, true, false);
    }

    public void Reanudar()
    {
        estado = Estado.Jugando;
        Time.timeScale = 1f;
        MostrarPaneles(false, true, false, false);
    }

    void TerminarJuego()
    {
        estado = Estado.GameOver;
        Time.timeScale = 0f;
        inicioGameOver = Time.unscaledTime;

        if (puntos > record)
        {
            record = puntos;
            PlayerPrefs.SetInt("record", record);
            PlayerPrefs.Save();
        }

        textoFinal.text = "PUNTOS: " + puntos + "\nRÉCORD: " + record;
        MostrarPaneles(false, false, false, true);
        GestorAudio.DetenerMusica();
        GestorAudio.FinJuego();
    }

    public void Salir()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ------------------------------------------------------------------
    //  Puntos, vidas y niveles
    // ------------------------------------------------------------------
    public void SumarPuntos(int cantidad)
    {
        puntos += cantidad;

        int nuevoNivel = 1 + puntos / PuntosPorNivel;
        if (nuevoNivel > nivel)
        {
            nivel = nuevoNivel;
            MostrarBanner("NIVEL " + nivel);
            GestorAudio.Nivel();
        }
        ActualizarHUD();
    }

    public void PerderVida()
    {
        vidas--;
        GestorAudio.Golpe();
        ActualizarHUD();
        if (vidas <= 0) TerminarJuego();
    }

    public void RecogerEstrella()
    {
        GestorAudio.Estrella();
        estrellasRecogidas++;

        // Cada 5 estrellas: vida extra
        if (estrellasRecogidas % 5 == 0 && vidas < VidasMaximas)
        {
            vidas++;
            MostrarBanner("¡VIDA EXTRA!");
        }
        SumarPuntos(50);
    }

    // ------------------------------------------------------------------
    //  Aparicion de enemigos
    // ------------------------------------------------------------------
    void ActualizarSpawn()
    {
        temporizadorSpawn -= Time.deltaTime;
        if (temporizadorSpawn > 0f) return;

        // Mientras mas nivel, mas rapido aparecen
        temporizadorSpawn = Mathf.Max(0.3f, 1.0f - 0.07f * (nivel - 1));

        float x = Random.Range(-MitadAncho + 0.8f, MitadAncho - 0.8f);
        Vector3 posicion = new Vector3(x, MitadAlto + 1f, 0f);
        float velocidad = 3.5f + 0.6f * (nivel - 1) + Random.Range(0f, 1.5f);

        if (Random.value < 0.14f)
        {
            // Estrella
            GameObject go = Instantiate(prefabEstrella, posicion, Quaternion.identity, contenedor);
            go.GetComponent<Estrella>().velocidad = velocidad * 0.8f;
        }
        else
        {
            // Asteroide (tamaño al azar: los pequeños dan mas puntos, los grandes aguantan 2 balas)
            GameObject go = Instantiate(prefabAsteroide, posicion, Quaternion.identity, contenedor);
            float escala = Random.Range(0.7f, 1.6f);
            go.transform.localScale = new Vector3(escala, escala, escala);

            Asteroide a = go.GetComponent<Asteroide>();
            a.velocidad = velocidad;
            a.deriva = Random.Range(-1.2f, 1.2f);
            a.puntos = escala < 1f ? 20 : 10;
            a.golpes = escala > 1.3f ? 2 : 1;
        }
    }

    // ------------------------------------------------------------------
    //  Efectos y utilidades
    // ------------------------------------------------------------------
    public void Explosion(Vector3 posicion, Material material, int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            GameObject f = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(f.GetComponent<Collider>());
            f.transform.SetParent(contenedor);
            f.transform.position = posicion;
            float tam = Random.Range(0.1f, 0.3f);
            f.transform.localScale = new Vector3(tam, tam, tam);
            if (material != null) f.GetComponent<Renderer>().sharedMaterial = material;
            f.AddComponent<Fragmento>();
        }
    }

    void LimpiarContenedor()
    {
        for (int i = contenedor.childCount - 1; i >= 0; i--)
        {
            GameObject hijo = contenedor.GetChild(i).gameObject;
            hijo.SetActive(false);
            Destroy(hijo);
        }
    }

    void MostrarPaneles(bool menu, bool hud, bool pausa, bool gameOver)
    {
        panelMenu.SetActive(menu);
        panelHUD.SetActive(hud);
        panelPausa.SetActive(pausa);
        panelGameOver.SetActive(gameOver);
    }

    void MostrarBanner(string mensaje)
    {
        textoBanner.text = mensaje;
        textoBanner.gameObject.SetActive(true);
        temporizadorBanner = 1.5f;
    }

    void ActualizarBanner()
    {
        if (temporizadorBanner <= 0f) return;
        temporizadorBanner -= Time.deltaTime;
        if (temporizadorBanner <= 0f) textoBanner.gameObject.SetActive(false);
    }

    void ActualizarHUD()
    {
        textoPuntos.text = "PUNTOS: " + puntos;
        textoNivel.text = "NIVEL " + nivel;
        textoVidas.text = "VIDAS: " + vidas;
        textoRecord.text = "RÉCORD: " + Mathf.Max(record, puntos);
    }
}
