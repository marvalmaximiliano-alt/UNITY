// EfectoRayo.cs
// Rayo laser: aparece y se adelgaza hasta desaparecer.
using UnityEngine;

public class EfectoRayo : MonoBehaviour
{
    public float vida = 0.35f;

    float t;
    Vector3 escalaInicial;

    void Start()
    {
        escalaInicial = transform.localScale;
    }

    void Update()
    {
        t += Time.deltaTime;
        float k = Mathf.Clamp01(1f - t / vida);
        transform.localScale = new Vector3(escalaInicial.x * k, escalaInicial.y * k, escalaInicial.z);
        if (t >= vida) Destroy(gameObject);
    }
}
