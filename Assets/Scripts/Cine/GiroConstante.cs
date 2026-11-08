// GiroConstante.cs
// Hace girar un objeto todo el tiempo (planeta, estacion, asteroides, estrellas...).
using UnityEngine;

public class GiroConstante : MonoBehaviour
{
    public Vector3 velocidad = new Vector3(0f, 30f, 0f);   // grados por segundo en cada eje

    void Update()
    {
        transform.Rotate(velocidad * Time.deltaTime, Space.Self);
    }
}
