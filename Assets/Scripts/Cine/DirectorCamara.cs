// DirectorCamara.cs
// "Director de fotografia": contiene todos los movimientos de camara de la cinematica.
// Va en un objeto vacio (RigCamara) que tiene la camara como hija.
//   Corte        -> cambio brusco de posicion (corte de edicion)
//   Plano        -> travelling: la camara va de A a B mirando un punto fijo
//   PlanoSigue   -> igual, pero mirando a un objeto que se mueve
//   Orbitar      -> la camara gira alrededor de un objeto
//   Seguir       -> camara de persecucion detras de un objeto
//   SeguirMundo  -> camara paralela (con un desplazamiento fijo)
//   Temblar      -> sacudida de camara (explosiones, impactos)
// Ademas todos cambian el campo de vision (FOV) para dar sensacion de velocidad o dramatismo.
using System.Collections;
using UnityEngine;

public class DirectorCamara : MonoBehaviour
{
    public Camera cam;      // la camara hija

    float intensidad;
    float restante;
    float total = 1f;

    void Mirar(Vector3 desde, Vector3 hacia)
    {
        Vector3 d = hacia - desde;
        if (d.sqrMagnitude < 0.0001f) return;
        transform.rotation = Quaternion.LookRotation(d);
    }

    // ---------- Corte ----------
    public void Corte(Vector3 pos, Vector3 mirar, float fov)
    {
        transform.position = pos;
        Mirar(pos, mirar);
        cam.fieldOfView = fov;
    }

    // ---------- Travelling con punto fijo ----------
    public IEnumerator Plano(Vector3 desde, Vector3 hasta, Vector3 mirar, float dur, float fovIni, float fovFin)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float k = Animar.Suave(t / dur);
            transform.position = Vector3.Lerp(desde, hasta, k);
            Mirar(transform.position, mirar);
            cam.fieldOfView = Mathf.Lerp(fovIni, fovFin, k);
            yield return null;
        }
    }

    // ---------- Travelling mirando a un objeto en movimiento ----------
    public IEnumerator PlanoSigue(Vector3 desde, Vector3 hasta, Transform objetivo, float dur, float fovIni, float fovFin)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float k = Animar.Suave(t / dur);
            transform.position = Vector3.Lerp(desde, hasta, k);
            Mirar(transform.position, objetivo.position);
            cam.fieldOfView = Mathf.Lerp(fovIni, fovFin, k);
            yield return null;
        }
    }

    // ---------- Orbita alrededor de un objeto (angulos en grados) ----------
    public IEnumerator Orbitar(Transform centro, float radio, float altura, float angIni, float angFin, float dur, float fov)
    {
        float t = 0f;
        cam.fieldOfView = fov;
        while (t < dur)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(angIni, angFin, Animar.Suave(t / dur)) * Mathf.Deg2Rad;
            Vector3 c = centro.position;
            transform.position = c + new Vector3(Mathf.Cos(a) * radio, altura, Mathf.Sin(a) * radio);
            Mirar(transform.position, c);
            yield return null;
        }
    }

    // ---------- Camara de persecucion (detras del objeto, con retraso suave) ----------
    public IEnumerator Seguir(Transform objetivo, Vector3 offsetLocal, float dur, float fovIni, float fovFin)
    {
        transform.position = objetivo.TransformPoint(offsetLocal);
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            Vector3 meta = objetivo.TransformPoint(offsetLocal);
            transform.position = Vector3.Lerp(transform.position, meta, 1f - Mathf.Exp(-7f * Time.deltaTime));
            Mirar(transform.position, objetivo.position + objetivo.forward * 12f);
            cam.fieldOfView = Mathf.Lerp(fovIni, fovFin, Animar.Suave(t / dur));
            yield return null;
        }
    }

    // ---------- Camara paralela con desplazamiento fijo en el mundo ----------
    public IEnumerator SeguirMundo(Transform objetivo, Vector3 offset, float dur, float fovIni, float fovFin, Vector3 mirarDelante)
    {
        transform.position = objetivo.position + offset;
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            Vector3 meta = objetivo.position + offset;
            transform.position = Vector3.Lerp(transform.position, meta, 1f - Mathf.Exp(-9f * Time.deltaTime));
            Mirar(transform.position, objetivo.position + mirarDelante);
            cam.fieldOfView = Mathf.Lerp(fovIni, fovFin, Animar.Suave(t / dur));
            yield return null;
        }
    }

    // ---------- Sacudida ----------
    public void Temblar(float fuerza, float duracion)
    {
        intensidad = fuerza;
        restante = duracion;
        total = Mathf.Max(0.01f, duracion);
    }

    void LateUpdate()
    {
        if (restante > 0f)
        {
            restante -= Time.deltaTime;
            cam.transform.localPosition = Random.insideUnitSphere * intensidad * Mathf.Clamp01(restante / total);
        }
        else
        {
            cam.transform.localPosition = Vector3.zero;
        }
    }
}
