// EfectoExplosion.cs
// Explosion hecha con primitivos: una bola de fuego que crece y se encoge,
// trocitos (cubos) que salen disparados y una luz que se apaga.
using UnityEngine;

public class EfectoExplosion : MonoBehaviour
{
    Transform[] trozos;
    Vector3[] velocidades;
    Vector3[] escalas;
    Transform bola;
    Light luz;
    float vida;
    float t;
    float tam;

    public void Iniciar(string claveMaterial, float tamano, float duracion)
    {
        tam = tamano;
        vida = duracion;
        Material m = Mundo.Mat(claveMaterial);

        int n = 14;
        trozos = new Transform[n];
        velocidades = new Vector3[n];
        escalas = new Vector3[n];

        for (int i = 0; i < n; i++)
        {
            float e = tam * Random.Range(0.08f, 0.22f);
            GameObject g = Mundo.Prim(PrimitiveType.Cube, transform, Vector3.zero, new Vector3(e, e, e), m);
            g.transform.localRotation = Random.rotation;
            trozos[i] = g.transform;
            escalas[i] = g.transform.localScale;
            velocidades[i] = Random.onUnitSphere * tam * Random.Range(0.8f, 2.2f);
        }

        bola = Mundo.Prim(PrimitiveType.Sphere, transform, Vector3.zero, Vector3.zero, Mundo.Mat("chispa")).transform;

        luz = gameObject.AddComponent<Light>();
        luz.type = LightType.Point;
        luz.color = new Color(1f, 0.6f, 0.2f);
        luz.range = tam * 10f;
        luz.intensity = 0f;
    }

    void Update()
    {
        if (trozos == null) return;

        t += Time.deltaTime;
        float k = Mathf.Clamp01(t / vida);

        for (int i = 0; i < trozos.Length; i++)
        {
            trozos[i].localPosition += velocidades[i] * Time.deltaTime;
            trozos[i].localScale = escalas[i] * (1f - k);
        }

        float b = k < 0.25f ? k / 0.25f : 1f - (k - 0.25f) / 0.75f;
        bola.localScale = Vector3.one * tam * 1.4f * b;
        luz.intensity = 8f * (1f - k);

        if (t >= vida) Destroy(gameObject);
    }
}
