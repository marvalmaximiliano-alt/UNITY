# Guía paso a paso — Actividad N°1: Salón ILE-1 en Unity

**Entrega: domingo 9 de octubre de 2026, hasta las 11:59 p.m. No hay prórroga.**
(Tienes: la escena, los prefabs y un video — o un build para Windows.)

---

## PARTE 1 — Preparar Unity (10 min)

1. Abre **Unity Hub** → **New project** → plantilla **3D** (o "Universal 3D"; el script funciona con las dos) → ponle un nombre, por ejemplo `SalonILE1` → **Create**.
2. Espera a que abra el editor (la primera vez tarda).
3. Copia el archivo `ConstruirSalonILE1.cs` de este repositorio (carpeta `Assets/Editor/`) dentro de la carpeta **Assets** de tu proyecto, respetando la carpeta `Editor`:
   `TuProyecto/Assets/Editor/ConstruirSalonILE1.cs`
   (Puedes arrastrarlo desde el explorador de archivos hasta la ventana **Project** de Unity.)
4. Vuelve a Unity y espera a que termine de compilar (rueda girando abajo a la derecha).
   Si sale un error rojo en la **Console**, cópiamelo y lo arreglo.

## PARTE 2 — Construir el salón con un clic (1 min)

1. En la barra de menú de arriba aparece un menú nuevo: **ILE-1 → Construir salon completo**.
2. Haz clic. Se crea una escena nueva con:
   - Piso, paredes, pared de vidrio, techo, puerta con cartel azul
   - TV, pantalla de proyector, pizarra, papelera
   - 2 mesas largas, **16 computadoras** (monitor + teclado + mouse) y **16 sillas**
   - Luces
3. En **Hierarchy** verás todo ordenado dentro de `Salon_ILE1` (Estructura, Mobiliario, Equipos, Iluminacion).
4. En la ventana **Project** → `Assets/ILE1_Salon/` están:
   - `Prefabs/` → `MesaLarga`, `PuestoPC`, `Silla`, `TiraLED`  ← **estos son tus prefabs**
   - `Materiales/` → los colores
   - `Escenas/Salon_ILE1.unity` → la escena guardada

> Para ver el interior desde arriba: en Hierarchy abre `Salon_ILE1 → Estructura`, selecciona `Techo` y desmarca la casilla de arriba del Inspector (desactivarlo).
> Para moverte en la vista Scene: clic derecho mantenido + teclas **W A S D**.

## PARTE 3 — Personalizar (opcional, da puntos de creatividad/detalle)

Prueba cambiar cosas tú mismo para aprender (y poder explicarlas en el video):

- Cambia el color de una silla: selecciona la silla → en el Inspector, arrastra otro material.
- **Edita un prefab**: haz doble clic en `Silla` (Project) → cambia algo → vuelve atrás. **Todas** las sillas cambian. Eso es lo que quiere el profesor ver en "Manejo de prefabs".
- Añade detalles que se vean en las fotos: más monitores, un reloj, una segunda papelera, cables (cilindros delgados), una luz de color, etc. **Solo con primitivos** (Cube, Sphere, Cylinder, Capsule, Plane, Quad).
- Revisa que ningún objeto flote ni atraviese otro (rúbrica: "Distribución de los objetos", 6 pts).

## PARTE 4 — Entregar (video recomendado)

**Qué debes mostrar en el video (breve, 2–4 min):**
1. La escena en el editor: gira alrededor del salón en la vista Scene.
2. Los prefabs: muestra la carpeta `Prefabs` y abre uno (`PuestoPC`).
3. Explica cómo construiste los objetos. Guion sugerido:
   - *"El salón se compone solo de primitivos de Unity. Por ejemplo, la silla está hecha con cubos para el asiento y el respaldo, un cilindro como pistón y esferas pequeñas como ruedas."*
   - *"El puesto de computadora es un prefab: un cubo para la pantalla, otro para el teclado, una esfera achatada para el mouse. Lo coloqué 16 veces en vez de construir cada computadora."*
   - *"Si cambio el prefab, cambian todas las copias."*
   - *"Las paredes son cubos escalados; la pared izquierda usa un material transparente para simular el vidrio."*

**Cómo grabar (lo más fácil, en Windows):** presiona **Win + G** → botón de grabar. O usa OBS Studio (gratis).
Si el archivo pesa mucho, comprímelo en .zip.

**Subir a Aula Virtual.** Si no deja, súbelo a Google Drive y pon el enlace en la página de la actividad (comparte con el profesor y el preparador).

**Alternativa sin video — Build para Windows:**
`File → Build Profiles` (o `Build Settings`) → agrega la escena (`Add Open Scenes`) → plataforma **Windows** → **Build**. Comprime la carpeta resultante en .zip y entrégala.

---

### Rúbrica (para que no se te escape nada)
| Aspecto | Puntos | Qué hacer |
|---|---|---|
| Atención al detalle | 8 | Que se parezca al salón de las fotos (TV, pantalla, vidrio, mesas, sillas, computadoras, puerta…) |
| Distribución de objetos | 6 | Nada flotando ni atravesando superficies |
| Creatividad | 4 | Materiales, luces, detalles extra |
| Manejo de prefabs | 2 | Usar prefabs para repetir mesas/PC/sillas |
