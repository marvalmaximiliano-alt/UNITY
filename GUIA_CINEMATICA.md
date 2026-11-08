# Actividad 3 — Recursos creativos: cinemática de Star Rush

**Entrega: domingo 15 de noviembre de 2026, 11:59 p.m. (no hay prórroga).**
Se entrega un **video de la cinemática grabado desde Unity** (más de 1 minuto; la nuestra dura ~105 s).
Si es en equipo, sube un solo integrante e indica los nombres de todos.

## La cinemática: "STAR RUSH — El origen"
Es el tráiler narrativo del proyecto final (el juego Star Rush). Todo se mueve **por código en tiempo de ejecución**:
personajes (la nave Nova-1 y el jefe "El Devorador"), utilería (asteroides, estrellas, estación, planeta) y **cámara**.
Todo está hecho solo con **primitivos** de Unity.

| Tiempo | Plano | Cámara |
|---|---|---|
| 0–9 s | Título "STAR RUSH – EL ORIGEN" sobre el planeta Nova | Travelling hacia el planeta (FOV 50→45), fundido desde negro |
| 9–21 s | Planeta con anillo, luna y estación Aurora en órbita | Órbita alrededor del planeta |
| 21–33 s | La nave Nova-1 junto a la estación | Contrapicado que se acerca + grúa que sube |
| 33–42 s | ¡ALERTA! Campo de asteroides (luz roja y sirena) | Vista cenital con temblor |
| 42–45 s | Despegue de la nave | Plano fijo que gira siguiéndola, FOV 50→70 |
| 45–61 s | Persecución: la nave esquiva, dispara y recoge estrellas | Cámara detrás (FOV 60→78), paralela, contrapicado de paso, cámara pegada a los motores con temblor |
| 61–71 s | Aparece el jefe "EL DEVORADOR" | Dolly zoom (se aleja mientras el FOV baja 85→35) con temblor |
| 71–85 s | Batalla: láseres, giros de tonel, contraataque, explosión en cadena | Órbita alrededor de la nave + plano general, y cámara lenta en la explosión final |
| 85–97 s | La nave se aleja; "La galaxia está a salvo... por ahora." | Cámara que se retira siguiendo a la nave |
| 97–105 s | Fundido a negro, título y créditos | — |

**Animación (7 pts):** movimiento por código de nave, jefe, estación, luna, asteroides, estrellas, balas, láseres y explosiones;
giros de tonel, balanceo de la nave según su velocidad lateral, llamas que palpitan, estelas, luces que parpadean, ojo del jefe que late.
**Cámara (7 pts):** travelling, órbita, persecución, paralela, cenital, contrapicado, dolly zoom, cambios de FOV, temblor, cortes y cámara lenta.
**Creatividad (6 pts):** historia propia ligada al juego, música original de 105 s sincronizada con los planos, efectos de sonido (sirena, láser, rugido, motor, explosiones),
subtítulos, barras de cine, destellos, fundidos y créditos.

## Paso 1 — Importar (2 min)
1. Abre tu proyecto de Star Rush en Unity.
2. Descarga `StarRush_Cinematica.unitypackage`.
3. **Assets → Import Package → Custom Package…** → elígelo → **Import** (todo marcado).
4. Espera a que compile y revisa la pestaña **Console**: no debe haber errores rojos (los amarillos no importan). Si hay, mándamelos.

## Paso 2 — Construir la escena (1 min)
1. Menú **Arcade → Construir cinematica**.
2. Si pregunta por guardar la escena actual: **Don't Save**.
3. Pulsa **▶ Play** y mírala completa (dura unos 105 s). El modo Play se detiene solo cuando termina.
4. **Pon los nombres del equipo:** en la Hierarchy selecciona **Cinematica** → en el Inspector, **Creditos** → escribe los nombres (una línea por persona; puedes aumentar el tamaño de la lista).
5. Guarda con **Cmd + S**.

> Si ves el Game view muy pequeño o recortado, ponlo en **Free Aspect** o **Full HD (1920x1080)** en el selector de resolución del Game view.

## Paso 3 — Grabar el video DESDE UNITY (con sonido)
En Mac, Cmd+Shift+5 **no graba el sonido del juego**, así que usaremos **Unity Recorder**:
1. **Arcade → Instalar Unity Recorder (para grabar el video)** y espera a que termine (ruedita abajo a la derecha).
2. **Window → General → Recorder → Recorder Window**.
3. **+ Add Recorder → Movie**.
4. Configura:
   - **Source:** Game View
   - **Output Resolution:** FHD – 1080p (1920 x 1080) (pon también el Game view en Full HD)
   - **Output Format:** MP4 (H.264)
   - **Audio:** marcado (Include Audio)
   - **Output File:** elige el nombre y la carpeta (por ejemplo `Cinematica_StarRush`)
5. Pulsa **Start Recording**. Unity entra solo en Play, reproduce toda la cinemática y la graba.
6. Cuando termina (el Play se detiene solo), tu video queda en la carpeta `Recordings` del proyecto (o donde elegiste).
7. **Ábrelo y comprueba** que dura más de 1 minuto y que se oye la música.

## Paso 4 — Entregar
- Sube el `.mp4` al Aula Virtual (si pesa mucho, súbelo a Google Drive, compártelo con el profesor y el preparador y pega el enlace en la actividad).
- Escribe en la entrega los nombres de **todos** los integrantes.

## Cómo explicarlo (por si preguntan)
| Archivo (`Assets/Scripts/Cine`) | Qué hace |
|---|---|
| `Cinematica.cs` | El **guion**: lista los planos en orden con sus subtítulos, sonidos y efectos. |
| `DirectorCamara.cs` | Todos los **movimientos de cámara** (travelling, órbita, persecución, temblor, FOV). |
| `Animar.cs` | Mover, girar y desvanecer objetos o textos con el tiempo (tweens). |
| `Mundo.cs` | Construye los objetos con **primitivos** y sus materiales. |
| `EfectoExplosion`, `EfectoRayo`, `ProyectilCine` | Explosiones, láseres y disparos. |
| `GiroConstante`, `PulsoEscala`, `Parpadeo` | Giros, latidos y luces que parpadean. |

## Si algo sale mal
- **No aparece el menú Arcade → Construir cinematica:** mira la Console (errores en rojo) y mándame una captura.
- **Pantalla negra al darle Play:** abre la escena `Assets/StarRush/Escenas/Cinematica` y comprueba que existan los objetos **RigCamara** (con Main Camera dentro) y **Cinematica**.
- **Se ve muy oscuro o raro:** dímelo y ajusto luces o colores.
