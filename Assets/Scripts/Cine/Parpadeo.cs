// Parpadeo.cs
// Enciende y apaga un objeto (luces de la estacion).
using UnityEngine;

public class Parpadeo : MonoBehaviour
{
    public float periodo = 1f;
    public float fase = 0f;

    Renderer r;

    void Awake()
    {
        r = GetComponent<Renderer>();
    }

    void Update()
    {
        if (r != null) r.enabled = ((Time.time + fase) % periodo) < periodo * 0.5f;
    }
}
