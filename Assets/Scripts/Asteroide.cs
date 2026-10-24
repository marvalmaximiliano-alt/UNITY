// Asteroide.cs
// Cae desde arriba girando. Si lo toca una bala se rompe (los grandes aguantan 2 balas).
// Si toca la nave, la nave pierde una vida.
using UnityEngine;

public class Asteroide : MonoBehaviour
{
    public float velocidad = 4f;    // la fija el GameManager al crearlo
    public float deriva = 0f;       // movimiento lateral
    public int puntos = 10;
    public int golpes = 1;          // balas necesarias para destruirlo

    float giro;
    bool destruido;

    void Start()
    {
        giro = Random.Range(-90f, 90f);
    }

    void Update()
    {
        GameManager gm = GameManager.Instancia;

        transform.position += new Vector3(deriva, -velocidad, 0f) * Time.deltaTime;
        transform.Rotate(0f, 0f, giro * Time.deltaTime);

        // Rebota en los bordes laterales
        float limite = gm.MitadAncho - 0.5f;
        float x = transform.position.x;
        if ((x > limite && deriva > 0f) || (x < -limite && deriva < 0f)) deriva = -deriva;

        // Si cae por abajo, desaparece
        if (transform.position.y < -gm.MitadAlto - 2f) Destroy(gameObject);
    }

    void OnTriggerEnter(Collider otro)
    {
        if (destruido) return;

        // Choque con una bala
        Bala bala = otro.GetComponent<Bala>();
        if (bala != null)
        {
            Destroy(bala.gameObject);
            golpes--;
            if (golpes <= 0)
            {
                GameManager.Instancia.SumarPuntos(puntos);
                Romper();
            }
            else
            {
                GestorAudio.Explosion();
                GameManager.Instancia.Explosion(transform.position, MaterialVisual(), 3);
            }
            return;
        }

        // Choque con la nave
        Jugador jugador = otro.GetComponent<Jugador>();
        if (jugador != null)
        {
            jugador.Golpeado();
            Romper();
        }
    }

    void Romper()
    {
        destruido = true;
        GestorAudio.Explosion();
        GameManager.Instancia.Explosion(transform.position, MaterialVisual(), 10);
        Destroy(gameObject);
    }

    Material MaterialVisual()
    {
        Renderer r = GetComponentInChildren<Renderer>();
        return r != null ? r.sharedMaterial : null;
    }
}
