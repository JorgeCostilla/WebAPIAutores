# PROGRAMACIÓN ASINCRONA

Siempre se debe usar programación asincrona cuando trabajamos con una base de datos desde nuestra WebAPI. ¿Pero por que es esto?

Para eso primero debemos de entender ¿que es la programación asincrona?

## Veamos una analogia
Supongamos que estas en tu casa y tu pides una pizza. Te dicen que se van a tomar 30 minutos en llegar a tu casa. Que haces ahi te quedas fijado ahi esperando a que llegue la pizza? o te pones a realizar tareas en tu casa ya sea limpiar ver video u otra cosa productiva mientras esperas a que llegue la pizza. Despues cuando llegue la pizza puedes dejar de hacer lo que estas haciendo bajas a la puerta y recibes la pizza. Es sentido comun porque quieres aprovechar al maximo tu tiempo.

Es algo similar con la programacion asincrona. 

## Programacion asincrona
Cuando ejecutamos una funcion asincrona, nuestro servidor Web se pone a hacer otras tareas mientras la función se ejecuta.

Es importante que no siempre debemos hacer todas nuestras tareas con programacion asincronas. Nada es gratis en programacion y al hacer esto tiene un pequeño costo de rendimiento aunque siempre que lo usemos bien ese costo es menor al beneficio que le vamos a obtener.

## ¿Cuando debemos usar programacion asincrona?
Cuando realizamos operaciones I/O (estas son operaciones que realizamos fuera de nuestra aplicación). Algunos ejemplos son:

- Hacer llamados a Web API (Es decir que desde nuestro webApi podemos llamar a otro webAPi), leer archivos en una PC, realizar operaciones con bases de datos. 

## Entonces ¿por que es buena para usarla para realizar operaciones con bases de datos?
Una cosa es tu WebAPI y otra tu Base de datos. 

Cuando haces un query esta mandando una peticion de tu WebAPi a tu servidor de base de datos el cual se va a encargar de procesar ese query que tu estas mandando y despues de procesarla de pueda mandar una respuesta a tu webAPI pero mientras ese tiempo de procesamiento tu webAPI no tiene porque esperar esta puede hacer otras cosas.

Si no usas programacion asincrona cuando mas usuarios tengas sera mas lenta debido a que el hilo sera mas tardado y lo ideal es que ese hilo se libere en tu servidor.

Con la programacion asincrona se aumenta la eficiencia de la webAPI.


## Programacion Asincrona en C#
En c# para poder usar una funcion asincrona se debe usar **async** y dentro de esta funcion asincrona podemos y debemos usar el operador **await** que hace que esperamos de manera asincrona de la operacion. Este await indica que esta esperando la respuesta asi que puede hacer otra cosa y una vez que la reciba ya puede continuar con lo de abajo.

```c#
public async Task<ActionResult<Autor>> Get(int id, string param2)
{
    var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

    if (autor == null)
    {
        return NotFound();
    }
    return autor;
}
```

## Para poder usar la programacion asinconra correctamente necesitamos usar TASK o ValueTask
En este curso nos vamos a enfocar unicamente en Task. 

Task representa una promesa, es decir un valor que va a retornar en un futuro entonces por eso siempre se usa Task porque es necesario. Esto le permite a C# realizar otras tareas mientras se espera de una resolucion de una operacion como una llamada de una base de datos.

## Task vs Task de T
Task es para no retornar un valor al final de la ejecucion de la funcion asincrona.

Task de T es para retornar el tipo de T en el futuro.