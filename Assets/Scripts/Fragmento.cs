// Fragmento.cs
// Trocito de las explosiones: sale volando en una direccion al azar, gira, se encoge y desaparece.
using UnityEngine;

public class Fragmento : MonoBehaviour
{
    Vector3 direccion;
    Vector3 escalaInicial;
    float vida = 0.7f;
    float tiempo;

    void Start()
    {
        direccion = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized * Random.Range(2f, 6f);
        escalaInicial = transform.localScale;
    }

    void Update()
    {
        tiempo += Time.deltaTime;
        transform.position += direccion * Time.deltaTime;
        transform.Rotate(0f, 0f, 360f * Time.deltaTime);

        float k = Mathf.Clamp01(1f - tiempo / vida);
        transform.localScale = escalaInicial * k;

        if (tiempo >= vida) Destroy(gameObject);
    }
}
