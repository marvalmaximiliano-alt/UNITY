// Animar.cs
// Funciones de animacion reutilizables (tweens): mover, girar y desvanecer cosas con el tiempo.
// Se usan con StartCoroutine(...), por ejemplo:  StartCoroutine(Animar.Mover(objeto, destino, 2f));
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class Animar
{
    // Curva suave: empieza y termina despacio (0..1 -> 0..1)
    public static float Suave(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t * (3f - 2f * t);
    }

    // Mueve un objeto hasta "destino" en "duracion" segundos (con aceleracion suave)
    public static IEnumerator Mover(Transform t, Vector3 destino, float duracion)
    {
        Vector3 inicio = t.position;
        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            t.position = Vector3.Lerp(inicio, destino, Suave(tiempo / duracion));
            yield return null;
        }
        t.position = destino;
    }

    // Mueve de A a B; "potencia" mayor que 1 = arranca lento y acelera (como un despegue)
    public static IEnumerator MoverEntre(Transform t, Vector3 a, Vector3 b, float duracion, float potencia)
    {
        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float k = Mathf.Pow(Mathf.Clamp01(tiempo / duracion), potencia);
            t.position = Vector3.Lerp(a, b, k);
            yield return null;
        }
        t.position = b;
    }

    // Gira un objeto una cantidad de grados extra (por ejemplo 360 en Z = un giro completo)
    public static IEnumerator Girar(Transform t, Vector3 giroExtra, float duracion)
    {
        Quaternion inicio = t.localRotation;
        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float k = Suave(tiempo / duracion);
            t.localRotation = inicio * Quaternion.Euler(giroExtra * k);
            yield return null;
        }
        t.localRotation = inicio * Quaternion.Euler(giroExtra);
    }

    // Cambia la transparencia de una imagen o texto de la interfaz
    public static IEnumerator Alfa(Graphic g, float alfaFinal, float duracion)
    {
        Color c = g.color;
        float alfaInicial = c.a;
        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            c.a = Mathf.Lerp(alfaInicial, alfaFinal, Suave(tiempo / duracion));
            g.color = c;
            yield return null;
        }
        c.a = alfaFinal;
        g.color = c;
    }
}
