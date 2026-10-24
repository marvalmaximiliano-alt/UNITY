// Bala.cs
// El disparo de la nave: sube en linea recta y se destruye al salir de la pantalla.
// (Quien detecta el choque con el asteroide es el script Asteroide.)
using UnityEngine;

public class Bala : MonoBehaviour
{
    public float velocidad = 20f;

    void Update()
    {
        transform.position += Vector3.up * velocidad * Time.deltaTime;

        if (transform.position.y > GameManager.Instancia.MitadAlto + 1f)
            Destroy(gameObject);
    }
}
