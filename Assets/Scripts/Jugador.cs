// Jugador.cs
// La nave del jugador: se mueve de izquierda a derecha y dispara hacia arriba.
using UnityEngine;

public class Jugador : MonoBehaviour
{
    public float velocidad = 12f;       // unidades por segundo
    public float cadencia = 0.22f;      // segundos entre disparos
    public GameObject prefabBala;

    float proximoDisparo;
    float invulnerable;                 // tiempo que queda de invulnerabilidad tras un golpe
    Renderer[] renderers;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    // Deja la nave lista al empezar una partida
    public void Reiniciar()
    {
        gameObject.SetActive(true);
        invulnerable = 0f;
        proximoDisparo = 0f;
        transform.rotation = Quaternion.identity;
        transform.position = new Vector3(0f, -GameManager.Instancia.MitadAlto + 1.3f, 0f);
        MostrarRenderers(true);
    }

    void Update()
    {
        GameManager gm = GameManager.Instancia;
        if (gm == null || !gm.Jugando) return;

        // --- Movimiento ---
        float h = Entrada.Horizontal();
        float limite = gm.MitadAncho - 0.9f;
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x + h * velocidad * Time.deltaTime, -limite, limite);
        transform.position = p;
        transform.rotation = Quaternion.Euler(0f, 0f, -h * 15f);   // se inclina al moverse

        // --- Disparo ---
        if (Entrada.Disparar() && Time.time >= proximoDisparo)
        {
            proximoDisparo = Time.time + cadencia;
            Instantiate(prefabBala, transform.position + Vector3.up * 0.9f, Quaternion.identity, gm.Contenedor);
            GestorAudio.Disparo();
        }

        // --- Parpadeo mientras es invulnerable ---
        if (invulnerable > 0f)
        {
            invulnerable -= Time.deltaTime;
            MostrarRenderers(invulnerable <= 0f || ((int)(invulnerable * 12f)) % 2 == 0);
        }
    }

    // Lo llama un asteroide cuando choca con la nave
    public void Golpeado()
    {
        if (invulnerable > 0f) return;
        invulnerable = 1.5f;
        GameManager.Instancia.PerderVida();
    }

    void MostrarRenderers(bool visible)
    {
        for (int i = 0; i < renderers.Length; i++) renderers[i].enabled = visible;
    }
}
