// Mundo.cs
// "Fabrica" de objetos de la cinematica. Todo se construye SOLO con primitivos de Unity
// (cubos, esferas, cilindros, capsulas) y se les da color con materiales creados por codigo.
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class Mundo
{
    // La nave del heroe: raiz (se mueve y apunta), modelo (se inclina), llamas y estelas
    public class Nave
    {
        public Transform raiz;
        public Transform modelo;
        public PulsoEscala[] llamas;
        public TrailRenderer[] estelas;

        public void Llamas(float k)
        {
            for (int i = 0; i < llamas.Length; i++) llamas[i].multiplicador = k;
        }

        public void Estelas(bool encendidas)
        {
            for (int i = 0; i < estelas.Length; i++)
            {
                estelas[i].Clear();
                estelas[i].emitting = encendidas;
            }
        }
    }

    static Dictionary<string, Material> cache = new Dictionary<string, Material>();
    static Shader shLit;
    static Shader shUnlit;

    // ------------------------------------------------------------------
    //  MATERIALES
    // ------------------------------------------------------------------
    static void BuscarShaders()
    {
        if (shLit != null && shUnlit != null) return;
        bool urp = GraphicsSettings.currentRenderPipeline != null;

        shLit = urp ? Shader.Find("Universal Render Pipeline/Lit") : null;
        if (shLit == null) shLit = Shader.Find("Standard");

        shUnlit = urp ? Shader.Find("Universal Render Pipeline/Unlit") : null;
        if (shUnlit == null) shUnlit = Shader.Find("Unlit/Color");
    }

    // Material con luz y sombras (con un poco de brillo propio opcional)
    public static Material Lit(Color c, float emision, float brillo)
    {
        BuscarShaders();
        Material m = new Material(shLit);
        m.color = c;
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", brillo);
        if (m.HasProperty("_Glossiness")) m.SetFloat("_Glossiness", brillo);
        if (emision > 0f)
        {
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", c * emision);
        }
        return m;
    }

    // Material "neon": no depende de la luz (siempre brilla igual)
    public static Material Neon(Color c)
    {
        BuscarShaders();
        Material m = new Material(shUnlit);
        m.color = c;
        return m;
    }

    // Paleta de colores de toda la cinematica
    public static Material Mat(string clave)
    {
        Material m;
        if (cache.TryGetValue(clave, out m) && m != null) return m;

        switch (clave)
        {
            case "casco": m = Lit(new Color(0.2f, 0.85f, 1f), 0.12f, 0.5f); break;
            case "cabina": m = Lit(new Color(0.9f, 0.97f, 1f), 0.5f, 0.8f); break;
            case "metal": m = Lit(new Color(0.55f, 0.58f, 0.65f), 0f, 0.6f); break;
            case "metalOscuro": m = Lit(new Color(0.18f, 0.18f, 0.22f), 0f, 0.5f); break;
            case "negro": m = Lit(new Color(0.03f, 0.03f, 0.04f), 0f, 0.2f); break;
            case "panel": m = Lit(new Color(0.15f, 0.25f, 0.6f), 0.05f, 0.8f); break;
            case "roca": m = Lit(new Color(0.9f, 0.4f, 0.28f), 0.08f, 0.15f); break;
            case "rocaGris": m = Lit(new Color(0.5f, 0.45f, 0.42f), 0f, 0.1f); break;
            case "crater": m = Lit(new Color(0.35f, 0.12f, 0.1f), 0f, 0.05f); break;
            case "planeta": m = Lit(new Color(0.1f, 0.5f, 0.65f), 0.05f, 0.3f); break;
            case "tierra": m = Lit(new Color(0.2f, 0.6f, 0.35f), 0.03f, 0.1f); break;
            case "nube": m = Lit(new Color(0.95f, 0.97f, 1f), 0.1f, 0.1f); break;
            case "anillo": m = Lit(new Color(0.85f, 0.75f, 0.55f), 0.05f, 0.2f); break;
            case "luna": m = Lit(new Color(0.6f, 0.6f, 0.65f), 0f, 0.1f); break;
            case "jefe": m = Lit(new Color(0.2f, 0.17f, 0.27f), 0.02f, 0.5f); break;
            case "jefe2": m = Lit(new Color(0.32f, 0.1f, 0.16f), 0.03f, 0.4f); break;
            case "naranja": m = Neon(new Color(1f, 0.5f, 0.1f)); break;
            case "llama": m = Neon(new Color(1f, 0.55f, 0.15f)); break;
            case "amarillo": m = Neon(new Color(1f, 0.9f, 0.2f)); break;
            case "explosion": m = Neon(new Color(1f, 0.5f, 0.1f)); break;
            case "chispa": m = Neon(new Color(1f, 0.95f, 0.6f)); break;
            case "rayo": m = Neon(new Color(1f, 0.2f, 0.2f)); break;
            case "luzRoja": m = Neon(new Color(1f, 0.15f, 0.1f)); break;
            case "ojo": m = Neon(new Color(1f, 0.3f, 0.1f)); break;
            case "luzVerde": m = Neon(new Color(0.2f, 1f, 0.4f)); break;
            case "luzAzul": m = Neon(new Color(0.3f, 0.7f, 1f)); break;
            case "estrellaBlanca": m = Neon(new Color(0.9f, 0.95f, 1f)); break;
            case "estrellaAzul": m = Neon(new Color(0.6f, 0.75f, 1f)); break;
            case "estrellaAmarilla": m = Neon(new Color(1f, 0.95f, 0.7f)); break;
            default: m = Neon(Color.magenta); break;
        }
        cache[clave] = m;
        return m;
    }

    // ------------------------------------------------------------------
    //  PRIMITIVO BASICO (sin collider: no hace falta fisica)
    // ------------------------------------------------------------------
    public static GameObject Prim(PrimitiveType tipo, Transform padre, Vector3 pos, Vector3 esc, Material mat, Vector3 rot, string nombre)
    {
        GameObject go = GameObject.CreatePrimitive(tipo);
        Object.Destroy(go.GetComponent<Collider>());
        if (nombre != null) go.name = nombre;
        go.transform.SetParent(padre, false);
        go.transform.localPosition = pos;
        go.transform.localEulerAngles = rot;
        go.transform.localScale = esc;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        return go;
    }

    public static GameObject Prim(PrimitiveType tipo, Transform padre, Vector3 pos, Vector3 esc, Material mat)
    {
        return Prim(tipo, padre, pos, esc, mat, Vector3.zero, null);
    }

    // ------------------------------------------------------------------
    //  CIELO, PLANETA Y ESTACION
    // ------------------------------------------------------------------
    public static void CrearEstrellasFondo(Transform padre)
    {
        string[] claves = { "estrellaBlanca", "estrellaAzul", "estrellaAmarilla" };
        GameObject grupo = new GameObject("EstrellasFondo");
        grupo.transform.SetParent(padre, false);
        for (int i = 0; i < 500; i++)
        {
            Vector3 dir = Random.onUnitSphere;
            float d = Random.Range(900f, 1500f);
            float e = Random.Range(3f, 9f);
            Prim(PrimitiveType.Sphere, grupo.transform, dir * d, new Vector3(e, e, e), Mat(claves[Random.Range(0, 3)]));
        }
    }

    // Particulas de polvo en el campo de asteroides (dan sensacion de velocidad)
    public static void CrearPolvo(Transform padre, Vector3 zona)
    {
        GameObject g = new GameObject("Polvo");
        g.transform.SetParent(padre, false);
        Material m = Mat("estrellaBlanca");
        for (int i = 0; i < 170; i++)
        {
            float ang = Random.value * Mathf.PI * 2f;
            float rad = Random.Range(6f, 45f);
            float e = Random.Range(0.15f, 0.45f);
            Vector3 pos = zona + new Vector3(Mathf.Cos(ang) * rad, Mathf.Sin(ang) * rad, Random.Range(-60f, 430f));
            Prim(PrimitiveType.Sphere, g.transform, pos, new Vector3(e, e, e), m);
        }
    }

    // Planeta "Nova" con continentes, nubes y anillo + luna
    public static Transform CrearPlaneta(Transform padre, out Transform luna)
    {
        GameObject raiz = new GameObject("Planeta");
        raiz.transform.SetParent(padre, false);

        GameObject rotador = new GameObject("Rotador");
        rotador.transform.SetParent(raiz.transform, false);
        rotador.AddComponent<GiroConstante>().velocidad = new Vector3(0f, 3f, 0f);

        Prim(PrimitiveType.Sphere, rotador.transform, Vector3.zero, new Vector3(80f, 80f, 80f), Mat("planeta"), Vector3.zero, "Nova");

        for (int i = 0; i < 12; i++)
        {
            Vector3 dir = Random.onUnitSphere;
            float tamano = Random.Range(12f, 26f);
            GameObject p = Prim(PrimitiveType.Sphere, rotador.transform, dir * 40.2f, new Vector3(tamano, tamano, 1.5f), Mat("tierra"));
            p.transform.localRotation = Quaternion.FromToRotation(Vector3.forward, dir);
        }
        for (int i = 0; i < 10; i++)
        {
            Vector3 dir = Random.onUnitSphere;
            float tamano = Random.Range(8f, 20f);
            GameObject p = Prim(PrimitiveType.Sphere, rotador.transform, dir * 41.2f, new Vector3(tamano, tamano * 0.5f, 1f), Mat("nube"));
            p.transform.localRotation = Quaternion.FromToRotation(Vector3.forward, dir);
        }

        Prim(PrimitiveType.Cylinder, raiz.transform, Vector3.zero, new Vector3(150f, 0.3f, 150f), Mat("anillo"), new Vector3(20f, 0f, 10f), "Anillo");

        luna = Prim(PrimitiveType.Sphere, padre, new Vector3(110f, 20f, -30f), new Vector3(14f, 14f, 14f), Mat("luna"), Vector3.zero, "Luna").transform;
        return raiz.transform;
    }

    // Estacion espacial "Aurora": nucleo, torre, aro de segmentos, radios, paneles solares y luces
    public static Transform CrearEstacion(Transform padre)
    {
        GameObject raiz = new GameObject("EstacionAurora");
        raiz.transform.SetParent(padre, false);
        Transform t = raiz.transform;
        Material metal = Mat("metal");

        Prim(PrimitiveType.Sphere, t, Vector3.zero, new Vector3(6f, 6f, 6f), metal, Vector3.zero, "Nucleo");
        Prim(PrimitiveType.Cylinder, t, Vector3.zero, new Vector3(3f, 9f, 3f), metal, Vector3.zero, "Torre");
        Prim(PrimitiveType.Cube, t, Vector3.zero, new Vector3(26f, 0.5f, 0.5f), metal, Vector3.zero, "RadioX");
        Prim(PrimitiveType.Cube, t, Vector3.zero, new Vector3(0.5f, 0.5f, 26f), metal, Vector3.zero, "RadioZ");

        for (int i = 0; i < 16; i++)
        {
            float ang = i * 22.5f;
            float r = ang * Mathf.Deg2Rad;
            Material mat = (i % 4 == 0) ? Mat("luzAzul") : metal;
            Prim(PrimitiveType.Cube, t, new Vector3(Mathf.Cos(r), 0f, Mathf.Sin(r)) * 13f,
                 new Vector3(3.6f, 1.4f, 2.2f), mat, new Vector3(0f, -(ang + 90f), 0f), "Segmento" + i);
        }

        for (int i = 0; i < 8; i++)
        {
            float r = i * 45f * Mathf.Deg2Rad + 0.39f;
            GameObject luz = Prim(PrimitiveType.Sphere, t, new Vector3(Mathf.Cos(r), 1.1f, Mathf.Sin(r)) * 13f,
                                  new Vector3(0.9f, 0.9f, 0.9f), Mat(i % 2 == 0 ? "luzVerde" : "luzRoja"), Vector3.zero, "Luz" + i);
            Parpadeo p = luz.AddComponent<Parpadeo>();
            p.periodo = 1.2f;
            p.fase = i * 0.3f;
        }

        Prim(PrimitiveType.Cube, t, new Vector3(0f, 9.5f, 0f), new Vector3(22f, 0.5f, 0.5f), metal);
        Prim(PrimitiveType.Cube, t, new Vector3(-8f, 9.5f, 0f), new Vector3(9f, 0.2f, 5f), Mat("panel"));
        Prim(PrimitiveType.Cube, t, new Vector3(8f, 9.5f, 0f), new Vector3(9f, 0.2f, 5f), Mat("panel"));

        raiz.transform.rotation = Quaternion.Euler(12f, 0f, 8f);
        raiz.AddComponent<GiroConstante>().velocidad = new Vector3(0f, 6f, 0f);
        return t;
    }

    // ------------------------------------------------------------------
    //  NAVE DEL HEROE: Nova-1 (la misma de Star Rush, ahora en 3D). La punta apunta a +Z.
    // ------------------------------------------------------------------
    public static Nave CrearNave(Transform padre)
    {
        Nave n = new Nave();

        GameObject raiz = new GameObject("Nova1");
        raiz.transform.SetParent(padre, false);
        GameObject modelo = new GameObject("Modelo");
        modelo.transform.SetParent(raiz.transform, false);
        Transform m = modelo.transform;
        Material casco = Mat("casco");

        Prim(PrimitiveType.Cube, m, Vector3.zero, new Vector3(1.3f, 0.9f, 4.2f), casco, Vector3.zero, "Cuerpo");
        Prim(PrimitiveType.Capsule, m, new Vector3(0f, 0f, 2.6f), new Vector3(0.9f, 0.9f, 0.9f), casco, new Vector3(90f, 0f, 0f), "Morro");
        Prim(PrimitiveType.Sphere, m, new Vector3(0f, 0.55f, 0.4f), new Vector3(0.95f, 0.75f, 1.7f), Mat("cabina"), Vector3.zero, "Cabina");
        Prim(PrimitiveType.Cube, m, new Vector3(-2.2f, 0f, -0.8f), new Vector3(3.2f, 0.12f, 1.8f), casco, new Vector3(0f, -20f, 0f), "AlaIzq");
        Prim(PrimitiveType.Cube, m, new Vector3(2.2f, 0f, -0.8f), new Vector3(3.2f, 0.12f, 1.8f), casco, new Vector3(0f, 20f, 0f), "AlaDer");
        Prim(PrimitiveType.Cube, m, new Vector3(-3.7f, 0.2f, -1.3f), new Vector3(0.15f, 0.5f, 0.8f), Mat("naranja"), new Vector3(0f, -20f, 0f), "PuntaIzq");
        Prim(PrimitiveType.Cube, m, new Vector3(3.7f, 0.2f, -1.3f), new Vector3(0.15f, 0.5f, 0.8f), Mat("naranja"), new Vector3(0f, 20f, 0f), "PuntaDer");
        Prim(PrimitiveType.Cube, m, new Vector3(0f, 0.8f, -1.6f), new Vector3(0.12f, 1.1f, 1.3f), casco, Vector3.zero, "Aleta");

        n.llamas = new PulsoEscala[2];
        n.estelas = new TrailRenderer[2];
        for (int i = 0; i < 2; i++)
        {
            float x = (i == 0) ? -0.6f : 0.6f;
            Prim(PrimitiveType.Cylinder, m, new Vector3(x, -0.05f, -2.4f), new Vector3(0.55f, 0.5f, 0.55f), Mat("metal"), new Vector3(90f, 0f, 0f), "Motor" + i);

            GameObject llama = Prim(PrimitiveType.Cylinder, m, new Vector3(x, -0.05f, -3.9f), new Vector3(0.35f, 1f, 0.35f), Mat("llama"), new Vector3(90f, 0f, 0f), "Llama" + i);
            PulsoEscala p = llama.AddComponent<PulsoEscala>();
            p.amplitud = 0.25f;
            p.frecuencia = 35f;
            n.llamas[i] = p;

            GameObject eg = new GameObject("Estela" + i);
            eg.transform.SetParent(m, false);
            eg.transform.localPosition = new Vector3(x, -0.05f, -4.8f);
            TrailRenderer tr = eg.AddComponent<TrailRenderer>();
            tr.time = 0.6f;
            tr.startWidth = 0.8f;
            tr.endWidth = 0f;
            tr.sharedMaterial = Mat("llama");
            tr.emitting = false;
            n.estelas[i] = tr;
        }

        n.raiz = raiz.transform;
        n.modelo = m;
        return n;
    }

    // ------------------------------------------------------------------
    //  ASTEROIDES, ESTRELLAS Y JEFE
    // ------------------------------------------------------------------
    public static GameObject CrearAsteroide(Transform padre, Vector3 pos, float escala)
    {
        GameObject raiz = new GameObject("Asteroide");
        raiz.transform.SetParent(padre, false);
        raiz.transform.localPosition = pos;

        Vector3 forma = new Vector3(Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f)) * escala;
        Material roca = Mat(Random.value < 0.65f ? "roca" : "rocaGris");
        Prim(PrimitiveType.Sphere, raiz.transform, Vector3.zero, forma, roca);
        for (int i = 0; i < 3; i++)
        {
            Vector3 d = Random.onUnitSphere;
            Prim(PrimitiveType.Sphere, raiz.transform, Vector3.Scale(d, forma) * 0.46f, forma * 0.28f, Mat("crater"));
        }

        raiz.transform.rotation = Random.rotation;
        raiz.AddComponent<GiroConstante>().velocidad = Random.onUnitSphere * Random.Range(10f, 40f);
        return raiz;
    }

    // Estrella coleccionable (dos cubos girados 45 grados, igual que en el juego)
    public static GameObject CrearEstrella(Transform padre, Vector3 pos)
    {
        GameObject raiz = new GameObject("Estrella");
        raiz.transform.SetParent(padre, false);
        raiz.transform.localPosition = pos;
        Material m = Mat("amarillo");
        Prim(PrimitiveType.Cube, raiz.transform, Vector3.zero, new Vector3(2f, 2f, 0.5f), m, Vector3.zero, "Cuadrado1");
        Prim(PrimitiveType.Cube, raiz.transform, Vector3.zero, new Vector3(2f, 2f, 0.5f), m, new Vector3(0f, 0f, 45f), "Cuadrado2");
        raiz.AddComponent<GiroConstante>().velocidad = new Vector3(0f, 0f, 120f);
        return raiz;
    }

    // El Devorador: nave nodriza enemiga con un enorme ojo rojo
    public static GameObject CrearJefe(Transform padre, out Transform aro, out Transform ojo)
    {
        GameObject raiz = new GameObject("Devorador");
        raiz.transform.SetParent(padre, false);
        Transform t = raiz.transform;

        Prim(PrimitiveType.Sphere, t, Vector3.zero, new Vector3(34f, 9f, 34f), Mat("jefe"), Vector3.zero, "Casco");
        Prim(PrimitiveType.Sphere, t, new Vector3(0f, 5f, 0f), new Vector3(14f, 12f, 14f), Mat("jefe2"), Vector3.zero, "Cupula");
        Prim(PrimitiveType.Sphere, t, new Vector3(0f, -3.5f, 0f), new Vector3(18f, 5f, 18f), Mat("jefe2"), Vector3.zero, "Vientre");

        GameObject a = new GameObject("Aro");
        a.transform.SetParent(t, false);
        a.AddComponent<GiroConstante>().velocidad = new Vector3(0f, 25f, 0f);
        for (int i = 0; i < 24; i++)
        {
            float ang = i * 15f;
            float r = ang * Mathf.Deg2Rad;
            Prim(PrimitiveType.Cube, a.transform, new Vector3(Mathf.Cos(r), 0f, Mathf.Sin(r)) * 16.5f,
                 new Vector3(2.4f, 1.3f, 1.3f), Mat("luzRoja"), new Vector3(0f, -(ang + 90f), 0f), "LuzAro" + i);
        }

        GameObject o = Prim(PrimitiveType.Sphere, t, new Vector3(0f, -0.5f, -15.5f), new Vector3(9f, 9f, 5f), Mat("ojo"), Vector3.zero, "Ojo");
        PulsoEscala pulso = o.AddComponent<PulsoEscala>();
        pulso.amplitud = 0.1f;
        pulso.frecuencia = 4f;
        pulso.uniforme = true;
        Prim(PrimitiveType.Sphere, t, new Vector3(0f, -0.5f, -17.5f), new Vector3(3.2f, 6f, 1.6f), Mat("negro"), Vector3.zero, "Pupila");

        Prim(PrimitiveType.Cube, t, new Vector3(-9f, -2f, -12f), new Vector3(2f, 2f, 10f), Mat("metalOscuro"), Vector3.zero, "CanonIzq1");
        Prim(PrimitiveType.Cube, t, new Vector3(9f, -2f, -12f), new Vector3(2f, 2f, 10f), Mat("metalOscuro"), Vector3.zero, "CanonDer1");
        Prim(PrimitiveType.Cube, t, new Vector3(-15f, -1f, -6f), new Vector3(2f, 2f, 10f), Mat("metalOscuro"), Vector3.zero, "CanonIzq2");
        Prim(PrimitiveType.Cube, t, new Vector3(15f, -1f, -6f), new Vector3(2f, 2f, 10f), Mat("metalOscuro"), Vector3.zero, "CanonDer2");
        Prim(PrimitiveType.Cube, t, new Vector3(-24f, 2f, 0f), new Vector3(22f, 1f, 12f), Mat("jefe"), new Vector3(0f, 0f, 10f), "AlaIzq");
        Prim(PrimitiveType.Cube, t, new Vector3(24f, 2f, 0f), new Vector3(22f, 1f, 12f), Mat("jefe"), new Vector3(0f, 0f, -10f), "AlaDer");

        raiz.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
        aro = a.transform;
        ojo = o.transform;
        return raiz;
    }

    // ------------------------------------------------------------------
    //  EFECTOS: explosion, proyectil y rayo laser
    // ------------------------------------------------------------------
    public static void Explosion(Vector3 pos, string claveMaterial, float tamano, float duracion)
    {
        GameObject go = new GameObject("Explosion");
        go.transform.position = pos;
        go.AddComponent<EfectoExplosion>().Iniciar(claveMaterial, tamano, duracion);
    }

    public static void CrearProyectil(Vector3 desde, Transform objetivo, float velocidad, System.Action alLlegar)
    {
        GameObject go = new GameObject("Proyectil");
        go.transform.position = desde;
        Prim(PrimitiveType.Capsule, go.transform, Vector3.zero, new Vector3(0.35f, 1.4f, 0.35f), Mat("amarillo"), new Vector3(90f, 0f, 0f), "Bala");
        ProyectilCine p = go.AddComponent<ProyectilCine>();
        p.objetivo = objetivo;
        p.velocidad = velocidad;
        p.alLlegar = alLlegar;
    }

    public static void Rayo(Vector3 a, Vector3 b, float grosor, float vida)
    {
        Vector3 d = b - a;
        if (d.sqrMagnitude < 0.01f) return;
        GameObject go = Prim(PrimitiveType.Cube, null, (a + b) * 0.5f, new Vector3(grosor, grosor, d.magnitude), Mat("rayo"), Vector3.zero, "Rayo");
        go.transform.rotation = Quaternion.LookRotation(d);
        go.AddComponent<EfectoRayo>().vida = vida;
    }
}
