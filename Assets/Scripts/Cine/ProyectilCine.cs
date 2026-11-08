// ProyectilCine.cs
// Disparo de la cinematica: vuela hacia un objetivo y, al llegar, ejecuta una accion (explosion).
using UnityEngine;

public class ProyectilCine : MonoBehaviour
{
    public Transform objetivo;
    public float velocidad = 120f;
    public System.Action alLlegar;

    void Update()
    {
        if (objetivo == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 d = objetivo.position - transform.position;
        float paso = velocidad * Time.deltaTime;

        if (d.magnitude <= paso + 0.5f)
        {
            if (alLlegar != null) alLlegar();
            Destroy(gameObject);
            return;
        }

        transform.position += d.normalized * paso;
        transform.rotation = Quaternion.LookRotation(d);
    }
}
