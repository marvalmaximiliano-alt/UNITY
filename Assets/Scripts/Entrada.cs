// Entrada.cs
// Lee el teclado. Funciona tanto con el "Input System" nuevo de Unity 6
// como con el sistema de entrada antiguo, asi no importa como este configurado tu proyecto.
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public static class Entrada
{
    // -1 = izquierda, 0 = quieto, +1 = derecha (flechas o A/D)
    public static float Horizontal()
    {
        float v = 0f;
#if ENABLE_INPUT_SYSTEM
        Keyboard k = Keyboard.current;
        if (k != null)
        {
            if (k.leftArrowKey.isPressed || k.aKey.isPressed) v -= 1f;
            if (k.rightArrowKey.isPressed || k.dKey.isPressed) v += 1f;
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) v -= 1f;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) v += 1f;
#endif
        return v;
    }

    // Espacio mantenido presionado = disparar
    public static bool Disparar()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard k = Keyboard.current;
        return k != null && k.spaceKey.isPressed;
#elif ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKey(KeyCode.Space);
#else
        return false;
#endif
    }

    // Enter o Espacio (justo en este frame): empezar / reintentar
    public static bool ConfirmarPresionado()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard k = Keyboard.current;
        return k != null && (k.enterKey.wasPressedThisFrame || k.numpadEnterKey.wasPressedThisFrame || k.spaceKey.wasPressedThisFrame);
#elif ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space);
#else
        return false;
#endif
    }

    // P o Escape (justo en este frame): pausa
    public static bool PausaPresionada()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard k = Keyboard.current;
        return k != null && (k.pKey.wasPressedThisFrame || k.escapeKey.wasPressedThisFrame);
#elif ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape);
#else
        return false;
#endif
    }
}
