# El Caballero Perdido

Un platformer vertical 2D en pixel art, hecho en Unity como proyecto universitario.

Un caballero despierta después de una batalla larguísima y no recuerda nada. Lo único que
lleva encima es un mapa incompleto que debería llevarlo de vuelta a casa. Para reconstruirlo
tiene que trepar: saltar de plataforma en plataforma mientras el mundo se hunde debajo suyo,
recogiendo los fragmentos que faltan y esquivando lo que se le cruce en el camino.

---

## Cómo se juega

El objetivo es simple de enunciar y difícil de cumplir: **subir**. La cámara asciende sola,
sin pausa y sin volver atrás nunca. Si el jugador se queda abajo, la cámara lo deja atrás y
cae al vacío. Si sube más rápido que la cámara, la cámara lo sigue.

Repartidos por el nivel están los **fragmentos de mapa**. Hay que recogerlos todos para
completar la partida. En el HUD se ve en todo momento cuántos van y cuántos faltan, además
de las vidas restantes.

El caballero empieza con **3 vidas**. Las pierde al caer fuera de pantalla, al chocar contra
un obstáculo o al recibir el ataque de una abeja enemiga. Cuando cae al vacío no vuelve a la
última plataforma pisada — esa posición ya quedó fuera de pantalla — sino que reaparece a
media altura de la cámara, con unos segundos de invulnerabilidad y gravedad reducida para
darle tiempo a reacomodarse. Al llegar a cero vidas, la partida termina.

### Controles

| Acción | Teclas |
|---|---|
| Moverse | `A` / `D` o flechas izquierda y derecha |
| Saltar | `W` o flecha arriba |
| Atacar | Clic izquierdo del mouse |

El ataque es cuerpo a cuerpo, funciona tanto en el aire como en el suelo y tiene un pequeño
enfriamiento entre golpe y golpe.

---

## Dificultades y finales

Hay dos modos de juego, elegibles desde el menú principal:

- **Fácil** — carga la escena `GameF`.
- **Difícil** — carga la escena `GameH`, con la cámara más agresiva y el terreno menos
  indulgente.

El progreso se guarda con `PlayerPrefs`, así que el menú recuerda entre sesiones qué
dificultades ya fueron superadas: los botones de los niveles completados aparecen tachados
y atenuados.

De ahí salen **tres finales distintos**, todos resueltos dentro de la misma escena `GameEnd`
mediante paneles superpuestos:

1. **Sin vidas** — el caballero no llegó. Se puede reintentar el mismo nivel o volver al menú.
2. **Nivel completo** — se juntaron todos los fragmentos de esa dificultad.
3. **Volviste a casa** — el final bueno, reservado para quien complete las dos dificultades.
   El mapa está entero y el caballero por fin sabe hacia dónde ir.

---

## Cómo está construido

El proyecto está pensado para que dos personas puedan trabajar en paralelo sin pisarse. La
frontera entre ambas mitades son interfaces y campos públicos, nunca ediciones cruzadas de
código ajeno.

**Estructura de escenas:** `MainMenu` (índice 0) → `GameF` o `GameH` según la dificultad
elegida → `GameEnd` → de vuelta al menú. Un `GameManager` con `DontDestroyOnLoad` sobrevive
a los cambios de escena y es quien recuerda qué dificultad se eligió y a qué escena hay que
volver al reintentar.

**Animación por script.** No se usa el Animator de Unity para el jugador: un componente
propio, `SpriteAnimator`, recorre arrays de `Sprite[]` con su propio frame rate y control de
loop. Es reutilizable para cualquier entidad y hace que la animación sea inspeccionable y
depurable desde código.

**Sistema de daño por interfaz.** Todo lo que puede recibir golpes implementa `IDamageable`
con un único método `TakeDamage(int)`. El jugador golpea con un `OverlapCircleAll` sobre la
capa de peligros y le pega a lo que encuentre, sin saber ni importarle qué es. Los enemigos
hacen lo propio contra el jugador. Ninguna de las dos partes necesita conocer la
implementación de la otra.

**Generación procedural.** Las plataformas no están colocadas a mano: se generan por delante
de la cámara y se destruyen cuando quedan suficientemente atrás, para no acumular objetos en
memoria. El generador está separado en piezas con responsabilidades claras — un selector que
elige el prefab con pesos y rarezas, un validador que comprueba que la nueva plataforma sea
alcanzable desde alguna anterior, y un generador de obstáculos que además verifica que
siempre quede una ruta viable por izquierda o por derecha.

**Audio centralizado.** Un `AudioManager` singleton maneja la música con transiciones suaves
entre pistas y persiste los volúmenes en `PlayerPrefs`. Cada escena declara su propia música
con un pequeño componente `SceneMusic`, y la pantalla final elige la pista según el desenlace.
Los sonidos de botón pasan por un script puente (`ButtonClickSound`) que resuelve el singleton
en tiempo de ejecución, porque las referencias directas del `OnClick()` a objetos de escena se
rompen al recargar.

---

## Scripts principales

**Jugador y cámara**
- `PlayerController` — movimiento, salto con *coyote time*, ground check, flip del sprite y
  límite lateral para que no se salga de la pantalla.
- `PlayerAttack` — ataque cuerpo a cuerpo con hitbox y enfriamiento.
- `PlayerHealth` — vidas, invulnerabilidad, respawn por altura de cámara, derrota.
- `SpriteAnimator` — animación por arrays de sprites.
- `CameraFollow` — ascenso automático que nunca retrocede y sigue al jugador si sube más rápido.
- `DeathZone` — el borde inferior que cobra la caída.

**Estados de juego, menús y audio**
- `GameManager` — singleton persistente; dificultad seleccionada y ruteo de escenas.
- `MainMenuController` — arranque de partida según dificultad.
- `DifficultButtonState` — tacha los niveles ya completados en el menú.
- `LevelCompleteChecker` — detecta la victoria leyendo el contador de fragmentos.
- `GameEndController` — decide qué panel final mostrar y qué música suena.
- `AudioManager`, `SceneMusic`, `ButtonClickSound` — capa de audio.
- `LivesCounter` — HUD de vidas.

**Enemigos, recolectables y generación**
- `EnemyBee`, `BeeAttackDamage` — enemigo con patrulla, persecución, ataque y muerte.
- `Mapa`, `MapaCounter` — fragmentos de mapa y su contador en pantalla.
- `PlatformGenerator`, `PlatformSelector`, `PlatformValidator`, `PlatformData` — generación
  procedural de plataformas.
- `PlatformObstacleGenerator`, `ObstacleDamage`, `ObstacleVerticalMovement`, `Flotar` —
  obstáculos y su comportamiento.

---

## Cómo ejecutarlo

**El ejecutable.** Descargá el `.zip` de la carpeta `Ejecutable/` (o desde la sección
*Releases*), descomprimilo entero y ejecutá el `.exe`. Importante: el `.exe` y la carpeta
`_Data` que lo acompaña tienen que quedar juntos y con sus nombres originales, o el juego no
arranca. Está pensado para Windows, en ventana de 1280×720.

**Desde el editor.** Abrí el proyecto con **Unity 6000.5.3f1**, cargá la escena `MainMenu`
y dale a Play. Las cuatro escenas (`MainMenu`, `GameF`, `GameH`, `GameEnd`) deben estar
listadas y tildadas en el Build Profile, con `MainMenu` en el índice 0.

---

## Herramientas usadas

Unity 6 y C# para el juego. Aseprite para el pixel art y los sprites del caballero.
TextMeshPro con la tipografía *Press Start 2P* para toda la interfaz. jsfxr para los efectos
de sonido y música libre de derechos para las pistas de fondo. Git y GitHub para el control
de versiones, con ramas por funcionalidad y pull requests hacia `main`.

Todos los sprites se importan con **Filter Mode = Point** y **Compression = None**, sin lo
cual el pixel art se ve borroso.

---

## Reparto del trabajo

| Área | Responsable |
|---|---|
| Jugador (movimiento, salto, ataque, vidas) | Eddy |
| Sistema de animación por sprites | Eddy |
| Cámara ascendente y zona de muerte | Eddy |
| Menús, estados de juego y navegación entre escenas | Eddy |
| Guardado de progreso con PlayerPrefs | Eddy |
| Audio (música, SFX, AudioManager) | Eddy |
| Interfaz y arte pixel art | Eddy |
| Enemigos y sistema de ataque enemigo | Leonidas |
| Recolectables y contador de fragmentos | Leonidas |
| Generación procedural de plataformas y obstáculos | Leonidas |
