# DELU-Proy

Proyecto Trimestral de la agrupación DELU de la USB

## Reglas de Organización
Juntar objetos (ej, scripts, arte, etc) en carpetas comunes. Por ejemplo, todos los sprites de un mismo enemigo deben estar en una misma carpeta y tener nombres descriptivos de lo que son. Para los scripts por ejemplo, si se tiene un script de movimiento de humanoide y vida de humanoide, ambos deberían estar en una carpeta llamada humanoide.

## Reglas de Push

* Siempre hacer pull antes de comenzar cualquier trabajo, esto es con el fin de evitar trabajar con cosas que han sido remplazadas.
* Hacer commits de cambios pequeños, ya que si algo se rompe es mejor arreglar un commit pequeño, que ver cientos de archivos a la vez. 
* Hacer push constantemente de los cambios. En caso de que rompan mucho el juego mientras se trabajan, se pueden crear branches y luego se unen a master, esto no aplica para escenas.
* Colocar titulos a los commits que expresen que se está cambiando. En caso de creer que sea necesario, agregar un texto completo describiendo que se hizo.
* Para probar nuevas cosas, es mejor crear escenas o duplicar escenas para trabajar. Una vez que se tenga la versión que se desea unir a las escenas finales:
  * Avisar al resto del grupo sobre cuales escenas se estan cambiando.
  * Trabajar en ellas lo más rápido posible para pushearlas.
  * Avisar al resto del grupo cuales escenas estan disponibles otra vez.
* Es recomendado trabajar con Prefabs, ya que ellos se pueden cambiar en escenas duplicadas o vacías y estos cambios se distribuyen automáticamente en el resto del proyecto.
* En caso de remplazar objetos binarios, por ejemplo, imágenes, videos, audio, etc... avisar que se está realizando este cambio para no chocar con otros artistas trabajando en ellos.


## Estándares de Código
El codigo tiene que tener nombres de variables con sentido en inglés y tiene que tener comentarios en español que ayuden a entender qué sucede.

Para comentar, se utilizan los comentarios XML (Ver [link](https://marketplace.visualstudio.com/items?itemName=k--kato.docomment))

## Herramientas
Se está usando la última versión de Unity 2019.
Para programar, se sugiere utilizar VS Code con los plugins para Unity y los comentarios tipo XML.