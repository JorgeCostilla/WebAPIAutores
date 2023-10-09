# INYECCIÓN DE DEPENDENCIAS

Las clases de nuestra aplicación rara vez son autosuficientes. Es normal separar responsabilidades entre nuestras distintas clases esto mismo ocurre en nuestros distintos controladores.

- Queremos tener clases con responsabilidades bien definidad.

Un controlador contiene un conjunto de acciones por lo que la responsabilidad del controlador es recibir peticiones HTTP realizadas a nuestra webAPI y coordinar el procesamiento de dicha peticion.

Sin embargo un controlador no ha de tener la responsabilidad de guardar registros en una base de datos ni de escribir en una consola ni nada por el estilo. Estas tareas deberán de ser delegadas a otras clases.

- Cuando una clase A utiliza una clase B, decimos que la clase B es una dependencia de la clase A.

Las dependencias son inebitables.

## Existen dos tipos de Acoplamientos

### Acoplamiento alto
Se caracteriza por una dependencia poco flexible en otras clases. (No es bueno en general).


### Acoplamiento bajo
Lo mejor que se puede hacer es esto y esto se implementa con Interfaces y servicios
Tiene un problema porque al hacer esto tendras dependencias sobre dependencias y la manera de resolver esto es el sistema de inyeccion de dependencias.