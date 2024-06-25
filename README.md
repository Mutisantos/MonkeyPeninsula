# PEC1 - Monkeys' Peninsula


## ¿Cómo Jugar?

Para iniciar el juego, dar click en el botón de Jugar, luego de esto los turnos se escogeran de manera aleatoria, por lo cual como jugador podrás o bien escoger un insulto o una réplica a un insulto recibido. Ganará quien pueda efectuar 3 ataques efectivos (donde el insultado no sea capaz de dar una réplica).


## Estructura Lógica del Proyecto

Para el control de la escena de Duelo de Insultos, se ha hecho una aproximación tratando de mantener las responsabilidades entre
componentes lo más separadas posibles, mientras que se mantiene la forma de interactuar entre dichos componentes. Los más relevantes son:

- GameManager: Instancia única Singleton que se encarga de estar presente en todas las escenas del juego. Su función principal es la de llevar control sobre el estado del juego, 
lo que incluye Puntuaciones, proveer los insultos y réplicas para los demás módulos y evaluar cuando una réplica es correcta o no. También facilita saber en que estado se encuentran
los combatientes, dado que esto afecta el comportamiento de otros componentes.
- StoryFlowManager: Encargado de hacer el manejo del flujo de dialogos y la interacción con los componentes del Canvas/UI del juego. Se aprovecha la implementación presente en StoryNodes para la generación de botones y manejo de listeners para que la selección de una opción efectue flujos de acuerdo con si es turno del enemigo o del jugador.
- CharactersHandler: Como su nombre indica, este componente se encarga de manejar a los combatientes en el juego y actualizar sus animaciones y posición dentro del mismo,
basado en la información de estados que le facilite el GameManager para así jugar con los componentes Animator del jugador y el enemigo, así como lo rigidBody2D para moverlos en 
el espacio de juego, apoyandose en Waypoints que marcan a que posiciones deben de moverse. 
- SoundManager: Utilizado para la reprodución de música de fondo y efectos de sonido, también accesible a través del resto de componentes.


## Estructura de Escenas

1. Pantalla Principal 
2. Escena de duelo de insultos
3. Pantalla de resultados
4. Pantalla de Créditos.

## Demo en Linea

[Demo disponible en Itch.io en este enlace](https://mutisantos.itch.io/monkeys-peninsula)

## Créditos

- Programación y Sprites: Esteban Hernández Losada
- Banda Sonora
--Vientos de Tatama: Jorge Velandia y Esteban Hernandez
--Combate de Boss: Jorge Velandia y Esteban Hernandez
--Tema de Victoria: Jorge Velandia y Esteban Hernandez

- Efectos de Sonido (Cursor2.ogg,094-Attack06.ogg,095-Attack07.ogg,096-Attack08.ogg,097-Attack09.ogg,100-Attack12.ogg)
-- Tomado de RPG:  Maker VX Ace Runtime Package.

- Fuentes
-- PixeloidSans : https://www.ggbot.net/fonts/
-- PressStart2P: http://www.zone38.net/font/ 
-- Pixelated: https://www.dafont.com/skylar-park.d2956
