// FondoEstrellas.cs
// Fondo animado: muchos puntitos que caen despacio para dar sensacion de viajar por el espacio.
// Usa unscaledDeltaTime para seguir moviéndose incluso en el menu (cuando el tiempo del juego esta detenido).
using UnityEngine;

public class FondoEstrellas : MonoBehaviour
{
    public Material material;
    public int cantidad = 70;

    Transform[] puntos;
    float[] velocidades;

    void Start()
    {
        GameManager gm = GameManager.Instancia;
        puntos = new Transform[cantidad];
        velocidades = new float[cantidad];

        for (int i = 0; i < cantidad; i++)
        {
            GameObject q = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(q.GetComponent<Collider>());
            q.transform.SetParent(transform);

            float tam = Random.Range(0.04f, 0.16f);
            q.transform.localScale = new Vector3(tam, tam, tam);
            q.transform.position = new Vector3(
                Random.Range(-gm.MitadAncho, gm.MitadAncho),
                Random.Range(-gm.MitadAlto, gm.MitadAlto),
                5f);
            q.GetComponent<Renderer>().sharedMaterial = material;

            puntos[i] = q.transform;
            velocidades[i] = tam * 25f;     // las mas grandes parecen mas cercanas y van mas rapido
        }
    }

    void Update()
    {
        GameManager gm = GameManager.Instancia;
        for (int i = 0; i < puntos.Length; i++)
        {
            Vector3 p = puntos[i].position;
            p.y -= velocidades[i] * Time.unscaledDeltaTime;
            if (p.y < -gm.MitadAlto - 0.5f)
            {
                p.y = gm.MitadAlto + 0.5f;
                p.x = Random.Range(-gm.MitadAncho, gm.MitadAncho);
            }
            puntos[i].position = p;
        }
    }
}
