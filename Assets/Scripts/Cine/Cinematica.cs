// Cinematica.cs
// STAR RUSH - "El origen"  (cortometraje animado, ~100 segundos)
//
// Todo se mueve por codigo en tiempo de ejecucion (Play):
//   - Mundo.cs construye los objetos con primitivos (planeta, estacion, nave, asteroides, jefe...)
//   - DirectorCamara.cs mueve la camara (travelling, orbitas, persecucion, temblor, cambios de FOV)
//   - Animar.cs mueve/gira/desvanece objetos e interfaz
//   - Aqui esta el GUION: la lista de planos en orden, con sus textos y sonidos.
//
// Escenas ("zonas") separadas en el espacio para poder hacer cortes limpios:
//   Planeta (origen) -> Estacion (300,0,0) -> Campo de asteroides (-300,0,0) -> Jefe (0,400,0)
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Cinematica : MonoBehaviour
{
    [Header("Referencias (las pone el constructor)")]
    public DirectorCamara director;
    public Light sol;

    [Header("Audio (las pone el constructor)")]
    public AudioClip clipMusica;
    public AudioClip clipAlarma;
    public AudioClip clipWhoosh;
    public AudioClip clipLaser;
    public AudioClip clipRugido;
    public AudioClip clipExplosion;
    public AudioClip clipDisparo;
    public AudioClip clipEstrella;
    public AudioClip clipMotor;

    [Header("Creditos finales (editalos!)")]
    public string[] creditos = { "Equipo: escribe aqui los nombres de los integrantes" };

    [Header("Opciones")]
    public bool detenerAlTerminar = true;   // para el modo Play solo cuando acaba (util al grabar)

    static readonly Vector3 ZonaEstacion = new Vector3(300f, 0f, 0f);
    static readonly Vector3 ZonaAsteroides = new Vector3(-300f, 0f, 0f);
    static readonly Vector3 ZonaJefe = new Vector3(0f, 400f, 0f);

    // segundos de vuelo en los que la nave dispara a los 3 asteroides grandes
    static readonly float[] TirosAsteroides = { 4.5f, 9f, 13.5f };
    // segundos de vuelo en los que la nave recoge cada estrella
    static readonly float[] TiemposEstrellas = { 2f, 6f, 8f, 11f, 15f };

    // Objetos
    Transform planeta, estacion, luna, jefeAro, jefeOjo;
    Mundo.Nave nave;
    GameObject jefe;
    GameObject[] objetivos = new GameObject[3];
    GameObject[] estrellasCampo = new GameObject[5];

    // Audio
    AudioSource srcMusica, srcEfectos;

    // Interfaz
    Text txtSub, txtTitulo, txtTitulo2, txtAlerta, txtEstrellas, txtCreditos;
    Image imgFundido, imgDestello, imgAlerta;

    // Estado
    bool orbitaEstacion;
    float anguloEstacion;
    bool flotarNave;
    Vector3 baseNave;
    int estrellasRecogidas;

    // =====================================================================
    //  INICIO
    // =====================================================================
    void Start()
    {
        Time.timeScale = 1f;
        ConfigurarEscena();
        CrearMundo();
        CrearInterfaz();

        srcMusica = gameObject.AddComponent<AudioSource>();
        srcMusica.clip = clipMusica;
        srcMusica.volume = 0.8f;
        srcMusica.playOnAwake = false;
        srcMusica.loop = false;

        srcEfectos = gameObject.AddComponent<AudioSource>();
        srcEfectos.playOnAwake = false;

        StartCoroutine(Guion());
    }

    void ConfigurarEscena()
    {
        Camera cam = director.cam;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.01f, 0.01f, 0.04f);
        cam.nearClipPlane = 0.3f;
        cam.farClipPlane = 4000f;

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.30f, 0.33f, 0.45f);

        if (sol != null)
        {
            sol.transform.rotation = Quaternion.LookRotation(new Vector3(0.4f, -0.3f, 1f));
            sol.intensity = 1.6f;
            sol.color = new Color(1f, 0.95f, 0.85f);
            sol.shadows = LightShadows.None;
        }
    }

    // Ruta que sigue la nave en el campo de asteroides (relativa a ZonaAsteroides)
    static Vector3 RutaNave(float t)
    {
        return new Vector3(5.5f * Mathf.Sin(0.8f * t), 2.5f * Mathf.Sin(0.55f * t + 1f), -40f + 28f * t);
    }

    void CrearMundo()
    {
        Transform mundo = new GameObject("Mundo").transform;

        Mundo.CrearEstrellasFondo(mundo);
        planeta = Mundo.CrearPlaneta(mundo, out luna);

        estacion = Mundo.CrearEstacion(mundo);
        anguloEstacion = 250f;
        float r0 = anguloEstacion * Mathf.Deg2Rad;
        estacion.position = new Vector3(120f * Mathf.Cos(r0), 6f, 120f * Mathf.Sin(r0));

        nave = Mundo.CrearNave(mundo);
        nave.raiz.gameObject.SetActive(false);

        // Campo de asteroides: un tunel largo; ninguna roca toca la ruta de la nave
        Transform campo = new GameObject("CampoAsteroides").transform;
        campo.SetParent(mundo, false);
        for (int i = 0; i < 120; i++)
        {
            float ang = Random.value * Mathf.PI * 2f;
            float rad = Random.Range(16f, 48f);
            float esc = rad < 26f ? Random.Range(1.5f, 3.5f) : Random.Range(2f, 7f);
            Vector3 rel = new Vector3(Mathf.Cos(ang) * rad, Mathf.Sin(ang) * rad, Random.Range(-60f, 430f));
            // dejamos libre el recorrido de la camara paralela para que ninguna roca tape la imagen
            if (rel.x > 8f && rel.x < 45f && Mathf.Abs(rel.y) < 14f && rel.z > 100f && rel.z < 240f) continue;
            Mundo.CrearAsteroide(campo, ZonaAsteroides + rel, esc);
        }
        Mundo.CrearPolvo(campo, ZonaAsteroides);

        // Los 3 asteroides grandes que la nave destruye (justo en su ruta)
        for (int i = 0; i < objetivos.Length; i++)
            objetivos[i] = Mundo.CrearAsteroide(campo, ZonaAsteroides + RutaNave(TirosAsteroides[i] + 2f), 4.5f);

        // Estrellas coleccionables sobre la ruta
        for (int i = 0; i < estrellasCampo.Length; i++)
            estrellasCampo[i] = Mundo.CrearEstrella(campo, ZonaAsteroides + RutaNave(TiemposEstrellas[i]));

        // El jefe (aparece mas adelante)
        jefe = Mundo.CrearJefe(mundo, out jefeAro, out jefeOjo);
        jefe.SetActive(false);
    }

    void Update()
    {
        if (orbitaEstacion && estacion != null)
        {
            anguloEstacion += 3.6f * Time.deltaTime;
            float r = anguloEstacion * Mathf.Deg2Rad;
            estacion.position = new Vector3(120f * Mathf.Cos(r), 6f, 120f * Mathf.Sin(r));
        }

        if (flotarNave && nave != null)
            nave.raiz.position = baseNave + Vector3.up * (Mathf.Sin(Time.time * 1.6f) * 0.35f);

        if (luna != null && planeta != null)
            luna.RotateAround(planeta.position, Vector3.up, 2f * Time.deltaTime);
    }

    // =====================================================================
    //  GUION (los planos de la cinematica, uno detras de otro)
    // =====================================================================
    IEnumerator Guion()
    {
        // ------------------------------------------------------------
        // PLANO 1 (0-9 s)  Titulo. Travelling hacia el planeta.
        // ------------------------------------------------------------
        imgFundido.color = new Color(0f, 0f, 0f, 1f);
        srcMusica.Play();
        director.Corte(new Vector3(0f, 10f, -420f), planeta.position, 50f);
        StartCoroutine(Animar.Alfa(imgFundido, 0f, 3f));
        Sfx(clipWhoosh, 0.6f);
        StartCoroutine(MostrarTitulo());
        yield return StartCoroutine(director.Plano(new Vector3(0f, 10f, -420f), new Vector3(0f, 28f, -200f), planeta.position, 9f, 50f, 45f));

        // ------------------------------------------------------------
        // PLANO 2 (9-21 s)  Orbita alrededor del planeta; la estacion tambien orbita.
        // ------------------------------------------------------------
        orbitaEstacion = true;
        StartCoroutine(Subtitular("Año 3026. La galaxia depende de una sola nave.", 5.5f, 1f));
        yield return StartCoroutine(director.Orbitar(planeta, 190f, 40f, 262f, 300f, 12f, 50f));
        orbitaEstacion = false;

        // ------------------------------------------------------------
        // PLANO 3 (21-33 s)  La estacion Aurora y la nave Nova-1 (contrapicado + grua).
        // ------------------------------------------------------------
        estacion.position = ZonaEstacion;
        Vector3 pNave = ZonaEstacion + new Vector3(20f, 0f, -12f);
        nave.raiz.gameObject.SetActive(true);
        nave.raiz.position = pNave;
        nave.raiz.rotation = Quaternion.identity;
        nave.modelo.localRotation = Quaternion.identity;
        nave.Llamas(0.6f);
        baseNave = pNave;
        flotarNave = true;

        StartCoroutine(Subtitular("Nova-1. La última nave de la flota estelar.", 5f, 0.5f));
        yield return StartCoroutine(director.PlanoSigue(pNave + new Vector3(-7f, -1.5f, -12f), pNave + new Vector3(-3f, 0.3f, -6.5f), nave.raiz, 6f, 55f, 40f));
        StartCoroutine(Subtitular("Su misión: proteger el Cinturón de Estrellas.", 5f, 0.3f));
        yield return StartCoroutine(director.PlanoSigue(pNave + new Vector3(6f, 1f, -9f), pNave + new Vector3(12f, 20f, -26f), nave.raiz, 6f, 45f, 65f));

        // ------------------------------------------------------------
        // PLANO 4 (33-42 s)  ALERTA. Vista cenital del campo de asteroides con temblor.
        // ------------------------------------------------------------
        StartCoroutine(Alarma());
        StartCoroutine(Subtitular("¡Campo de asteroides detectado!", 5f, 0.5f));
        Vector3 zb = ZonaAsteroides;
        director.Corte(zb + new Vector3(0f, 170f, 0f), zb + new Vector3(0f, 0f, 70f), 55f);
        director.Temblar(0.35f, 9f);
        yield return StartCoroutine(director.Plano(zb + new Vector3(0f, 170f, 0f), zb + new Vector3(0f, 70f, 40f), zb + new Vector3(0f, 0f, 70f), 9f, 55f, 55f));

        // ------------------------------------------------------------
        // PLANO 5a (42-45 s)  Despegue de la nave (camara fija que gira para seguirla).
        // ------------------------------------------------------------
        flotarNave = false;
        nave.Estelas(true);
        nave.Llamas(2.2f);
        Sfx(clipMotor, 0.9f);
        Sfx(clipWhoosh, 0.8f);
        director.Temblar(0.25f, 3f);
        Vector3 camDespegue = pNave + new Vector3(-12f, 2f, -16f);
        director.Corte(camDespegue, pNave, 50f);
        StartCoroutine(Animar.MoverEntre(nave.raiz, pNave, pNave + new Vector3(0f, 0f, 220f), 3.2f, 2.2f));
        yield return StartCoroutine(director.PlanoSigue(camDespegue, camDespegue + new Vector3(-6f, 1f, -6f), nave.raiz, 3.2f, 50f, 70f));

        // ------------------------------------------------------------
        // PLANO 5b (45-61 s)  Persecucion en el campo de asteroides.
        // ------------------------------------------------------------
        nave.Estelas(false);
        nave.Llamas(1.4f);
        nave.raiz.position = zb + RutaNave(0f);
        nave.raiz.rotation = Quaternion.LookRotation(Vector3.forward);
        nave.Estelas(true);

        estrellasRecogidas = 0;
        txtEstrellas.text = "ESTRELLAS: 0 / 5";
        StartCoroutine(Animar.Alfa(txtEstrellas, 1f, 0.5f));
        StartCoroutine(Subtitular("Esquiva. Dispara. Recoge estrellas.", 5f, 1.5f));
        StartCoroutine(VueloNave(16f));
        StartCoroutine(DisparosAsteroides());
        StartCoroutine(RecogerEstrellas());

        // camara 1: detras de la nave, con FOV que se abre (sensacion de velocidad)
        yield return StartCoroutine(director.Seguir(nave.raiz, new Vector3(0f, 3.5f, -11f), 5.5f, 60f, 78f));
        // camara 2: paralela, a un lado
        yield return StartCoroutine(director.SeguirMundo(nave.raiz, new Vector3(26f, 3f, -4f), 4f, 55f, 55f, new Vector3(0f, 0f, 5f)));
        // camara 3: contrapicado, la nave pasa justo al lado de la camara
        Vector3 camFrente = zb + RutaNave(11.2f) + new Vector3(-9f, -3.5f, 0f);
        yield return StartCoroutine(director.PlanoSigue(camFrente, camFrente, nave.raiz, 3.5f, 70f, 45f));
        // camara 4: muy cerca de los motores, FOV amplio y temblor
        director.Temblar(0.3f, 3f);
        yield return StartCoroutine(director.Seguir(nave.raiz, new Vector3(0f, 2f, -7f), 3f, 95f, 80f));
        StartCoroutine(Animar.Alfa(txtEstrellas, 0f, 0.5f));

        // ------------------------------------------------------------
        // PLANO 6 (61-71 s)  Aparece el jefe: dolly zoom, picado desde abajo, temblor.
        // ------------------------------------------------------------
        Vector3 zc = ZonaJefe;
        nave.Estelas(false);
        nave.Llamas(1f);
        nave.raiz.position = zc;
        nave.raiz.rotation = Quaternion.LookRotation(Vector3.forward);
        nave.modelo.localRotation = Quaternion.identity;

        jefe.SetActive(true);
        jefe.transform.position = zc + new Vector3(0f, 0f, 420f);
        Sfx(clipRugido, 1f);
        StartCoroutine(Animar.Mover(jefe.transform, zc + new Vector3(0f, 0f, 150f), 9f));
        StartCoroutine(Subtitular("Pero algo más antiguo ha despertado...", 5f, 1f));
        StartCoroutine(TituloJefe());
        director.Temblar(0.5f, 9f);
        Vector3 camJefe1 = zc + new Vector3(-10f, -4f, -22f);
        Vector3 camJefe2 = zc + new Vector3(-10f, -4f, -75f);
        yield return StartCoroutine(director.PlanoSigue(camJefe1, camJefe2, jefe.transform, 9.5f, 85f, 35f));

        // ------------------------------------------------------------
        // PLANO 7 (71-85 s)  Batalla: orbita alrededor de la nave + plano general.
        // ------------------------------------------------------------
        StartCoroutine(Subtitular("¡Es ahora o nunca!", 4f, 0.5f));
        StartCoroutine(BatallaJefe());
        yield return StartCoroutine(director.Orbitar(nave.raiz, 16f, 3f, -150f, -30f, 8f, 60f));
        yield return StartCoroutine(director.Plano(zc + new Vector3(120f, 14f, 70f), zc + new Vector3(105f, 10f, 85f), zc + new Vector3(0f, 0f, 75f), 5.5f, 65f, 65f));
        yield return StartCoroutine(ExplosionFinal());

        // ------------------------------------------------------------
        // PLANO 8 (85-97 s)  Epilogo: la nave se aleja, la camara se retira.
        // ------------------------------------------------------------
        nave.Llamas(2f);
        nave.Estelas(true);
        Vector3 pFin = nave.raiz.position;
        StartCoroutine(Animar.MoverEntre(nave.raiz, pFin, pFin + new Vector3(0f, 10f, 280f), 11f, 2f));
        StartCoroutine(Subtitular("La galaxia está a salvo... por ahora.", 5f, 1.5f));
        yield return StartCoroutine(director.PlanoSigue(pFin + new Vector3(-8f, 2f, -12f), pFin + new Vector3(-40f, 22f, -70f), nave.raiz, 11f, 55f, 40f));

        // ------------------------------------------------------------
        // CIERRE: fundido a negro y tarjeta final con el titulo y los creditos.
        // ------------------------------------------------------------
        yield return StartCoroutine(Animar.Alfa(imgFundido, 1f, 2f));
        txtTitulo.color = new Color(0.3f, 0.95f, 1f, 0f);
        txtTitulo.text = "STAR RUSH";
        txtTitulo2.text = "Esquiva. Dispara. Recoge estrellas.";
        txtCreditos.text = string.Join("\n", creditos);
        StartCoroutine(Animar.Alfa(txtTitulo, 1f, 1.2f));
        StartCoroutine(Animar.Alfa(txtTitulo2, 1f, 1.2f));
        yield return StartCoroutine(Animar.Alfa(txtCreditos, 1f, 1.2f));
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(Animar.Alfa(txtTitulo, 0f, 1f));

        Time.timeScale = 1f;
#if UNITY_EDITOR
        if (detenerAlTerminar) UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // =====================================================================
    //  ESCENAS PARALELAS (corren a la vez que la camara)
    // =====================================================================
    IEnumerator MostrarTitulo()
    {
        yield return new WaitForSeconds(1.2f);
        txtTitulo.text = "STAR RUSH";
        yield return StartCoroutine(Animar.Alfa(txtTitulo, 1f, 1.5f));
        yield return new WaitForSeconds(1.2f);
        txtTitulo2.text = "EL ORIGEN";
        yield return StartCoroutine(Animar.Alfa(txtTitulo2, 1f, 1f));
        yield return new WaitForSeconds(2.5f);
        StartCoroutine(Animar.Alfa(txtTitulo2, 0f, 1f));
        yield return StartCoroutine(Animar.Alfa(txtTitulo, 0f, 1f));
    }

    IEnumerator TituloJefe()
    {
        yield return new WaitForSeconds(5f);
        txtTitulo.color = new Color(1f, 0.2f, 0.2f, 0f);
        txtTitulo.text = "EL DEVORADOR";
        yield return StartCoroutine(Animar.Alfa(txtTitulo, 1f, 0.8f));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(Animar.Alfa(txtTitulo, 0f, 0.8f));
    }

    IEnumerator Alarma()
    {
        Sfx(clipAlarma, 0.8f);
        txtAlerta.text = "¡ALERTA!";
        float t = 0f;
        while (t < 8f)
        {
            t += Time.deltaTime;
            float p = (Mathf.Sin(t * 8f) + 1f) * 0.5f;
            imgAlerta.color = new Color(1f, 0f, 0f, 0.35f * p);
            txtAlerta.color = new Color(1f, 0.25f, 0.2f, p > 0.3f ? 1f : 0f);
            yield return null;
        }
        imgAlerta.color = new Color(1f, 0f, 0f, 0f);
        txtAlerta.color = new Color(1f, 0.25f, 0.2f, 0f);
    }

    // La nave vuela por la ruta (con balanceo segun la velocidad lateral)
    IEnumerator VueloNave(float dur)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            Vector3 p = ZonaAsteroides + RutaNave(t);
            Vector3 p2 = ZonaAsteroides + RutaNave(t + 0.1f);
            Vector3 fwd = (p2 - p).normalized;
            nave.raiz.position = p;
            nave.raiz.rotation = Quaternion.LookRotation(fwd);
            float lateral = (p2.x - p.x) / 0.1f;
            nave.modelo.localRotation = Quaternion.Euler(0f, 0f, -lateral * 4f);
            yield return null;
        }
    }

    // La nave dispara 2 balas a cada asteroide grande; la segunda lo hace explotar
    IEnumerator DisparosAsteroides()
    {
        float previo = 0f;
        for (int i = 0; i < TirosAsteroides.Length; i++)
        {
            yield return new WaitForSeconds(TirosAsteroides[i] - previo);
            previo = TirosAsteroides[i];

            GameObject objetivo = objetivos[i];
            for (int k = 0; k < 2; k++)
            {
                Sfx(clipDisparo, 0.7f);
                Vector3 origen = nave.raiz.TransformPoint(new Vector3(k == 0 ? -1.8f : 1.8f, 0f, 1.5f));
                if (k == 1) Mundo.CrearProyectil(origen, objetivo.transform, 140f, () => ExplotarObjetivo(objetivo));
                else Mundo.CrearProyectil(origen, objetivo.transform, 140f, null);
                yield return new WaitForSeconds(0.15f);
            }
        }
    }

    void ExplotarObjetivo(GameObject asteroide)
    {
        if (asteroide == null) return;
        Mundo.Explosion(asteroide.transform.position, "explosion", 9f, 1.2f);
        Sfx(clipExplosion, 1f);
        director.Temblar(0.5f, 0.5f);
        Destroy(asteroide);
    }

    IEnumerator RecogerEstrellas()
    {
        float previo = 0f;
        for (int i = 0; i < TiemposEstrellas.Length; i++)
        {
            yield return new WaitForSeconds(TiemposEstrellas[i] - previo);
            previo = TiemposEstrellas[i];

            if (estrellasCampo[i] != null)
            {
                Mundo.Explosion(estrellasCampo[i].transform.position, "amarillo", 3f, 0.6f);
                Destroy(estrellasCampo[i]);
            }
            Sfx(clipEstrella, 0.9f);
            estrellasRecogidas++;
            txtEstrellas.text = "ESTRELLAS: " + estrellasRecogidas + " / 5";
        }
    }

    // La nave esquiva con un giro de tonel
    IEnumerator Esquivar(float dx)
    {
        Vector3 a = nave.raiz.position;
        Vector3 b = a + new Vector3(dx, dx * 0.2f, 0f);
        StartCoroutine(Animar.Girar(nave.modelo, new Vector3(0f, 0f, -360f * Mathf.Sign(dx)), 0.9f));
        yield return StartCoroutine(Animar.Mover(nave.raiz, b, 0.45f));
        yield return StartCoroutine(Animar.Mover(nave.raiz, a, 0.45f));
    }

    void ImpactoJefe()
    {
        Mundo.Explosion(jefeOjo.position + Random.insideUnitSphere * 6f, "chispa", 6f, 0.6f);
        Sfx(clipExplosion, 0.35f);
        director.Temblar(0.25f, 0.2f);
    }

    // Duelo: el jefe dispara lasers, la nave esquiva y contraataca; el jefe estalla en cadena.
    IEnumerator BatallaJefe()
    {
        Vector3 posNave = nave.raiz.position;

        yield return new WaitForSeconds(1.5f);
        Sfx(clipLaser, 0.9f);
        Mundo.Rayo(jefeOjo.position, posNave + new Vector3(3f, 0f, 0f), 1.2f, 0.4f);
        StartCoroutine(Esquivar(-8f));

        yield return new WaitForSeconds(2f);
        Sfx(clipLaser, 0.9f);
        Mundo.Rayo(jefeOjo.position, posNave + new Vector3(-3f, 0f, 0f), 1.2f, 0.4f);
        StartCoroutine(Esquivar(8f));

        yield return new WaitForSeconds(2f);
        Sfx(clipLaser, 1f);
        Mundo.Rayo(jefeOjo.position + new Vector3(8f, 0f, 0f), posNave, 1.2f, 0.4f);
        Mundo.Rayo(jefeOjo.position + new Vector3(-8f, 0f, 0f), posNave, 1.2f, 0.4f);
        StartCoroutine(Esquivar(-7f));

        yield return new WaitForSeconds(1.5f);
        // contraataque: 10 disparos al ojo
        for (int i = 0; i < 10; i++)
        {
            Sfx(clipDisparo, 0.6f);
            Vector3 origen = nave.raiz.TransformPoint(new Vector3(i % 2 == 0 ? -1.8f : 1.8f, 0f, 1.5f));
            Mundo.CrearProyectil(origen, jefeOjo, 160f, () => ImpactoJefe());
            yield return new WaitForSeconds(0.12f);
        }

        yield return new WaitForSeconds(1.5f);
        // el jefe se descontrola: el aro gira cada vez mas rapido y hay explosiones en cadena
        jefeAro.GetComponent<GiroConstante>().velocidad = new Vector3(0f, 240f, 0f);
        for (int i = 0; i < 10; i++)
        {
            Mundo.Explosion(jefe.transform.position + Random.insideUnitSphere * 30f, "explosion", Random.Range(8f, 16f), 1.2f);
            Sfx(clipExplosion, 0.7f);
            director.Temblar(0.6f, 0.4f);
            yield return new WaitForSeconds(0.3f);
        }
    }

    // Explosion final con camara lenta y destello blanco
    IEnumerator ExplosionFinal()
    {
        Time.timeScale = 0.25f;
        Sfx(clipExplosion, 1f);
        Sfx(clipRugido, 0.6f);
        Vector3 c = jefe.transform.position;
        Mundo.Explosion(c, "explosion", 60f, 2.5f);
        Mundo.Explosion(c + new Vector3(0f, 10f, 0f), "chispa", 40f, 2f);
        jefe.SetActive(false);
        director.Temblar(1.2f, 1.5f);
        imgDestello.color = new Color(1f, 1f, 1f, 1f);
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(Animar.Alfa(imgDestello, 0f, 0.4f));
        yield return new WaitForSeconds(0.3f);
        Time.timeScale = 1f;
    }

    // =====================================================================
    //  UTILIDADES
    // =====================================================================
    void Sfx(AudioClip clip, float volumen)
    {
        if (clip != null) srcEfectos.PlayOneShot(clip, volumen);
    }

    IEnumerator Subtitular(string texto, float duracion, float retraso)
    {
        if (retraso > 0f) yield return new WaitForSeconds(retraso);
        txtSub.text = texto;
        yield return StartCoroutine(Animar.Alfa(txtSub, 1f, 0.4f));
        yield return new WaitForSeconds(Mathf.Max(0f, duracion - 0.8f));
        yield return StartCoroutine(Animar.Alfa(txtSub, 0f, 0.4f));
    }

    // =====================================================================
    //  INTERFAZ (subtitulos, titulos, barras de cine y fundidos)
    // =====================================================================
    void CrearInterfaz()
    {
        GameObject cgo = new GameObject("Canvas", typeof(RectTransform));
        Canvas canvas = cgo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler escalador = cgo.AddComponent<CanvasScaler>();
        escalador.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escalador.referenceResolution = new Vector2(1920f, 1080f);
        escalador.matchWidthOrHeight = 0.5f;
        Transform c = cgo.transform;

        Color cian = new Color(0.3f, 0.95f, 1f, 0f);
        Color blanco = new Color(1f, 1f, 1f, 0f);
        Color amarillo = new Color(1f, 0.9f, 0.2f, 0f);
        Color rojo = new Color(1f, 0.25f, 0.2f, 0f);

        // Orden: lo ultimo que se crea queda delante
        imgAlerta = NuevaImagen(c, "AlertaRoja", new Color(1f, 0f, 0f, 0f), true);
        imgDestello = NuevaImagen(c, "Destello", new Color(1f, 1f, 1f, 0f), true);

        txtTitulo = NuevoTexto(c, "Titulo", 150, cian, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0f, 70f), new Vector2(1700f, 200f));
        txtTitulo2 = NuevoTexto(c, "Subtitulo2", 60, blanco, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0f, -70f), new Vector2(1700f, 90f));
        txtAlerta = NuevoTexto(c, "Alerta", 130, rojo, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0f, 200f), new Vector2(1500f, 170f));
        txtSub = NuevoTexto(c, "Subtitulo", 48, blanco, TextAnchor.MiddleCenter, new Vector2(0.5f, 0f), new Vector2(0f, 150f), new Vector2(1700f, 110f));
        txtEstrellas = NuevoTexto(c, "Estrellas", 44, amarillo, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(60f, -140f), new Vector2(800f, 60f));
        txtCreditos = NuevoTexto(c, "Creditos", 40, blanco, TextAnchor.UpperCenter, new Vector2(0.5f, 0.5f), new Vector2(0f, -150f), new Vector2(1700f, 300f));

        // Barras negras de cine (formato panoramico)
        Image barraSup = NuevaImagen(c, "BarraSuperior", Color.black, false);
        RectTransform rs = barraSup.rectTransform;
        rs.anchorMin = new Vector2(0f, 1f);
        rs.anchorMax = new Vector2(1f, 1f);
        rs.pivot = new Vector2(0.5f, 1f);
        rs.anchoredPosition = Vector2.zero;
        rs.sizeDelta = new Vector2(0f, 110f);

        Image barraInf = NuevaImagen(c, "BarraInferior", Color.black, false);
        RectTransform ri = barraInf.rectTransform;
        ri.anchorMin = new Vector2(0f, 0f);
        ri.anchorMax = new Vector2(1f, 0f);
        ri.pivot = new Vector2(0.5f, 0f);
        ri.anchoredPosition = Vector2.zero;
        ri.sizeDelta = new Vector2(0f, 110f);

        imgFundido = NuevaImagen(c, "Fundido", Color.black, true);
    }

    Image NuevaImagen(Transform padre, string nombre, Color color, bool pantallaCompleta)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        Image img = go.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
        if (pantallaCompleta)
        {
            RectTransform rt = img.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
        return img;
    }

    Text NuevoTexto(Transform padre, string nombre, int tamano, Color color, TextAnchor alineacion, Vector2 ancla, Vector2 pos, Vector2 caja)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        Text t = go.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = tamano;
        t.alignment = alineacion;
        t.color = color;
        t.raycastTarget = false;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;

        RectTransform rt = t.rectTransform;
        rt.anchorMin = ancla;
        rt.anchorMax = ancla;
        rt.pivot = ancla;
        rt.anchoredPosition = pos;
        rt.sizeDelta = caja;

        Outline borde = go.AddComponent<Outline>();
        borde.effectColor = new Color(0f, 0f, 0f, 0.8f);
        borde.effectDistance = new Vector2(2f, -2f);
        return t;
    }
}
