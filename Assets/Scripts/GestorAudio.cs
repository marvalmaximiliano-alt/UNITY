// GestorAudio.cs
// Reproduce la musica de fondo (en bucle) y los efectos de sonido.
// Los sonidos se arrastran a los campos desde el Inspector (el constructor del juego ya lo hace).
using UnityEngine;

public class GestorAudio : MonoBehaviour
{
    public static GestorAudio Instancia;

    [Header("Musica")]
    public AudioClip clipMusica;

    [Header("Efectos de sonido")]
    public AudioClip clipDisparo;
    public AudioClip clipExplosion;
    public AudioClip clipEstrella;
    public AudioClip clipGolpe;
    public AudioClip clipNivel;
    public AudioClip clipFinJuego;

    AudioSource fuenteMusica;
    AudioSource fuenteEfectos;

    void Awake()
    {
        Instancia = this;

        fuenteMusica = gameObject.AddComponent<AudioSource>();
        fuenteMusica.clip = clipMusica;
        fuenteMusica.loop = true;
        fuenteMusica.volume = 0.35f;
        fuenteMusica.playOnAwake = false;

        fuenteEfectos = gameObject.AddComponent<AudioSource>();
        fuenteEfectos.playOnAwake = false;
    }

    void Start()
    {
        IniciarMusica();
    }

    void Efecto(AudioClip clip, float volumen)
    {
        if (clip != null) fuenteEfectos.PlayOneShot(clip, volumen);
    }

    // ---- Funciones estaticas para llamarlas desde cualquier script ----
    public static void IniciarMusica()
    {
        if (Instancia != null && Instancia.clipMusica != null && !Instancia.fuenteMusica.isPlaying)
            Instancia.fuenteMusica.Play();
    }

    public static void DetenerMusica()
    {
        if (Instancia != null) Instancia.fuenteMusica.Stop();
    }

    public static void Disparo()   { if (Instancia != null) Instancia.Efecto(Instancia.clipDisparo, 0.5f); }
    public static void Explosion() { if (Instancia != null) Instancia.Efecto(Instancia.clipExplosion, 0.8f); }
    public static void Estrella()  { if (Instancia != null) Instancia.Efecto(Instancia.clipEstrella, 0.9f); }
    public static void Golpe()     { if (Instancia != null) Instancia.Efecto(Instancia.clipGolpe, 1f); }
    public static void Nivel()     { if (Instancia != null) Instancia.Efecto(Instancia.clipNivel, 0.9f); }
    public static void FinJuego()  { if (Instancia != null) Instancia.Efecto(Instancia.clipFinJuego, 1f); }
}
