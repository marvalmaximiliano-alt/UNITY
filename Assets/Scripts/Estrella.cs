// Estrella.cs
// Objeto coleccionable: da puntos y cada 5 estrellas regala una vida extra.
using UnityEngine;

public class Estrella : MonoBehaviour
{
    public float velocidad = 3f;
    bool recogida;

    void Update()
    {
        transform.position += Vector3.down * velocidad * Time.deltaTime;
        transform.Rotate(0f, 0f, 120f * Time.deltaTime);

        if (transform.position.y < -GameManager.Instancia.MitadAlto - 2f) Destroy(gameObject);
    }

    void OnTriggerEnter(Collider otro)
    {
        if (recogida) return;
        if (otro.GetComponent<Jugador>() == null) return;

        recogida = true;
        GameManager.Instancia.RecogerEstrella();

        Renderer r = GetComponentInChildren<Renderer>();
        GameManager.Instancia.Explosion(transform.position, r != null ? r.sharedMaterial : null, 6);
        Destroy(gameObject);
    }
}
