// PulsoEscala.cs
// Hace "latir" el tamaño de un objeto (llamas de los motores, ojo del jefe...).
using UnityEngine;

public class PulsoEscala : MonoBehaviour
{
    public float amplitud = 0.2f;
    public float frecuencia = 12f;
    public float multiplicador = 1f;     // sirve para hacer la llama mas grande o mas pequeña
    public bool uniforme = false;        // true = crece en los 3 ejes; false = solo en el eje Y

    Vector3 escalaBase;
    float fase;

    void Awake()
    {
        escalaBase = transform.localScale;
        fase = Random.value * 6.28f;
    }

    void Update()
    {
        float s = multiplicador * (1f + amplitud * Mathf.Sin(Time.time * frecuencia + fase));
        if (uniforme) transform.localScale = escalaBase * s;
        else transform.localScale = new Vector3(escalaBase.x, escalaBase.y * s, escalaBase.z);
    }
}
