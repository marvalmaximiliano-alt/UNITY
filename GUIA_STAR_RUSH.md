# Star Rush — Actividad 2: Aspectos básicos de programación (25%)

**Entrega: domingo 25 de octubre de 2026, hasta las 11:59 p.m. (no hay prórroga).**
Entrega: un `.zip` con el **build para PC** + una carpeta con **todos los scripts** (`Scripts`).
Si es en equipo, la entrega la hace UN integrante e indica los nombres de todos.

## El juego
Arcade espacial original (no es Pong, no usa templates):
- **Controles:** A/D o flechas = mover · Espacio = disparar · P = pausa · Enter = empezar.
- **Objetivo:** esquivar y destruir asteroides. Los pequeños dan 20 puntos, los medianos 10 y los grandes (2 balas) 10.
- **Estrellas:** +50 puntos; cada 5 estrellas = 1 vida extra (máx. 5).
- **Vidas:** 3. Si un asteroide te toca pierdes una (hay 1.5 s de invulnerabilidad con parpadeo).
- **Niveles:** cada 300 puntos sube el nivel: los asteroides caen más rápido y aparecen más seguido.
- **Récord:** se guarda entre partidas (PlayerPrefs).
- **Menú principal con Canvas UI** (bonus): Jugar / Salir, pausa y pantalla de Game Over con botones.
- **Sonido:** música de fondo en bucle + 6 efectos (disparo, explosión, estrella, golpe, subir de nivel, game over).

## Paso 1 — Importar el juego a Unity (3 min)
1. En Unity Hub: **New project → Universal 3D** (o 3D) → crear. (Si prefieres, usa el proyecto que ya tienes.)
2. Descarga `StarRush.unitypackage`.
3. En Unity: menú **Assets → Import Package → Custom Package…** → elige el archivo → **Import** (deja todo marcado).
4. Espera a que termine de compilar (ruedita abajo a la derecha).
5. Abre la pestaña **Console**: no debe haber errores rojos. Si hay, mándame una captura.

## Paso 2 — Construir el juego con un clic
1. Arriba aparece el menú **Arcade**. Haz clic en **Arcade → Construir juego completo**.
2. Si pregunta por guardar la escena actual: **Don't Save**.
3. Aparece un aviso "Star Rush esta construido". Pulsa **OK**.
4. Pulsa el botón **▶ Play** (arriba, al centro). Pulsa **JUGAR** o Enter. ¡A jugar!
5. Guarda con **Cmd + S**.

> Si el mouse no hace clic en los botones, usa Enter para jugar y Espacio para reintentar: funciona igual.

## Paso 3 — Build para PC
La actividad pide un build **para PC** (Windows).
1. En **Unity Hub → Installs** → ⚙ de tu versión → **Add modules** → marca **Windows Build Support (Mono)** → Install. (Una sola vez.)
2. En Unity: **File → Build Profiles** (o Build Settings) → plataforma **Windows** → **Switch Platform** (espera).
3. Comprueba que la escena `StarRush` aparezca en la lista (el constructor ya la añade).
4. **Build** → elige una carpeta nueva (por ejemplo `StarRush_Build`) → espera.
5. Comprime esa carpeta completa en un `.zip`.

> Si no puedes instalar el módulo de Windows, pregunta al profesor si acepta build de macOS (Build Profiles → macOS).

## Paso 4 — Preparar el .zip final
Crea una carpeta `Entrega_StarRush` con:
- `Build/` → todo el contenido del build.
- `Scripts/` → copia aquí los 8 scripts de `Assets/Scripts` (y, si quieres, `Assets/Editor/ConstruirJuego.cs`).
- Un `.txt` con los nombres de los integrantes.

Comprímela en `.zip` y súbela al Aula Virtual.

## Cómo explicarlo (por si te preguntan)
| Script | Qué hace |
|---|---|
| `Entrada` | Lee el teclado (nuevo y viejo Input System). |
| `Jugador` | Mueve la nave, dispara, tiene invulnerabilidad al ser golpeado. |
| `Bala` | Sube en línea recta y se destruye fuera de la pantalla. |
| `Asteroide` | Cae girando, rebota en los bordes, se rompe con balas, daña a la nave. |
| `Estrella` | Coleccionable: puntos y vida extra. |
| `Fragmento` | Pedacitos de la explosión. |
| `FondoEstrellas` | Fondo animado. |
| `GestorAudio` | Música de fondo y efectos. |
| `GameManager` | Estados (menú/jugando/pausa/game over), puntos, vidas, niveles, aparición de enemigos, interfaz. |

Rúbrica: **Lógica** (inputs 3 + comportamiento 3) · **Jugabilidad** (mecánicas 3 + reglas/puntos 3) · **Creatividad** (6) · **Sonorización** (música 1 + efectos 1) · **Bonus** menú Canvas (+1).
