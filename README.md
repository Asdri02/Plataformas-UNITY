Práctica 1 – Plataformas en Unity
Descripción del proyecto
En esta práctica he desarrollado un pequeño juego de plataformas en 3D utilizando Unity (URP). El objetivo principal ha sido implementar un sistema de movimiento y salto con físicas básicas, junto con diferentes tipos de plataformas con comportamientos específicos.
He intentado no limitarme únicamente a lo mínimo exigido, sino mejorar la sensación de control del jugador y el comportamiento de las plataformas para que el resultado fuese más sólido y jugable.

1. Creación del jugador
Para el jugador creé un GameObject vacío llamado Player, al que añadí:
Un Capsule Collider ajustado a su forma.
Un Rigidbody con gravedad activada.
Bloqueo de rotaciones en los ejes necesarios para evitar que se tumbase al moverse.
El modelo visual se colocó como hijo del objeto principal para mantener separada la parte física de la parte gráfica.
El control del jugador está implementado en el script PlayerMotor. En él:
Obtengo la referencia al Rigidbody mediante GetComponent.
Leo el input horizontal y vertical (WASD).
Calculo la dirección de movimiento relativa a la orientación del jugador (usando transform.forward y transform.right).
Modifico la velocidad del Rigidbody de forma independiente del framerate usando Time.fixedDeltaTime.
Además, añadí rotación suave hacia la dirección del movimiento usando Quaternion.Slerp.

2. Sistema de salto
El salto está implementado con físicas reales utilizando AddForce con ForceMode.Impulse.
Para evitar saltos infinitos en el aire:
Uso un sistema de detección de suelo mediante Physics.CheckSphere.
Las plataformas están en la layer Ground, que utilizo como máscara (LayerMask).
Mantengo un booleano isGrounded.
Además, añadí mejoras opcionales para mejorar la sensación del salto:
Coyote time, que permite saltar durante unas milésimas tras abandonar el suelo.
Jump buffer, que guarda el input del salto si se pulsa justo antes de tocar el suelo.
Ajuste de gravedad al caer y al soltar el botón para que el salto tenga una sensación más natural.
Estas mejoras no eran obligatorias, pero mejoran notablemente el control del personaje.

3. Cámara
La cámara está configurada como hija del Player para crear una vista en tercera persona.
Ajusté manualmente su posición para que siga al jugador desde atrás. También modifiqué la sensibilidad de rotación para evitar movimientos bruscos.

4. Plataformas básicas
Las plataformas se crean siguiendo una estructura organizada:
Objeto padre vacío con collider.
Modelo visual como hijo.
Uso de prefabs para cada tipo de plataforma.
Todas las plataformas están correctamente etiquetadas con la layer Ground para que el jugador pueda detectarlas como suelo.
Se ha creado un pequeño recorrido jugable combinando diferentes alturas y distancias para obligar al jugador a saltar y desplazarse estratégicamente.

5. Plataformas con comportamiento
Tipo A – Plataforma móvil
Implementé un script MovingPlatformClase que:
Guarda la posición inicial.
Define una posición final mediante un segundo GameObject en la escena.
Se mueve usando Vector3.MoveTowards.
Utiliza Time.fixedDeltaTime para ser independiente del framerate.
Cambia de dirección al llegar a cada extremo.
Se añadieron varias instancias modificando velocidad y distancia desde el inspector.

Tipo B – Plataforma que cae
Implementé el script FallingPlatform, que:
Detecta al jugador mediante colisiones y comprobación de tag.
Inicia una cuenta atrás usando Time.time.
Tras el retardo, comienza a descender a velocidad constante.
Cuando alcanza una altura mínima, se detiene.
Espera un tiempo.
Vuelve a su posición inicial y se reinicia completamente.
Además, implementé la detección opcional de contacto solo desde arriba usando las normales de colisión.

Mejoras opcionales realizadas
Movimiento relativo a la orientación del jugador.
Rotación suave hacia la dirección de movimiento.
Coyote time.
Jump buffer.
Ajuste avanzado de gravedad para mejorar la sensación del salto.
Organización limpia del proyecto y uso de prefabs.

