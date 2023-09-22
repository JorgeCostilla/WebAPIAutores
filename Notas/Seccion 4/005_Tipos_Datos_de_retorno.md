# TIPOS DE DATOS DE RETORNO

Vamos a hablar de los tipos de datos de retorno de una accion.

Tenemos 3 opciones
- El tipo especifico
- El action result de t (t es cualquier cosa)
- Y un IActionResult

## Veamos el tipo especifico
En este ejemplo usamos programacion sincrona y es mala practica la buena practica es usar programacion asincrona cuando trabajamos con bases de datos.
```c#
public List<Autor> Get()
{
    return context.Autores.Include(x => x.Libros).ToList();
}
```

Cuando nos referimos a un tipo especifico nos referimos a un metodo que no es ActionResult ni IActionResult 


## Veamos el tipo ActionResult
En este ejemplo siguiente si quitamos el ActionResult el return NotFound() marcaria un error debido a que el metodo NotFound es un NotFoundResult.

```c#
[HttpGet("{id:int}/{param2=persona}")]
public ActionResult<Autor> Get(int id, string param2)
{
    var autor = await context.Autores.FirstOrDefault(x => x.Id == id);

    if (autor == null)
    {
        return NotFound();
    }
    return autor;
}
```

El ActionResult nos permite devolver la el tipo especificado en este caso Autor o un ActionResult o cualquier clase que sea deribada de un ActionResult.

NotFound hereda de ActionResult por eso se puede usar.


## Veamos el tipo IActionResult
Pareciera que no hay una gran diferencia, pero lo que falla es el return autor. Solo puedo retornar cualquier cosa que implemente IActionReturn.

¿Entonces que necesito para poder retornar un Autor? 

Necesito poner un return OK(autor). Hereda de un IActionResult entonces estoy retornarno un Http200 y el cuerpo de la respuesta sera lo que se coloque dentro de los parentesis.
```c#
[HttpGet("{id:int}/{param2=persona}")]
public IActionResult Get(int id, string param2)
{
    var autor = await context.Autores.FirstOrDefault(x => x.Id == id);

    if (autor == null)
    {
        return NotFound();
    }
    // Mal
    //return autor;

    // Bien
    return Ok(autor);
}
```

## Entonces cual es la diferencia entre ActionResult de t y IActionResult
Pues que con IActionResult tu no puedes controlar que lo que tu puedes controlar es un autor o el tipo que deseas porque puedes retornar un 7 o un otro tipo de dato. 

## ¿Cual es la razon de que existe los dos?
Porque ActionResult fue agregado despues en .Net Core 2.1 anteriormente solo existia IActionResult.

## Entonces en conclusion 
El maestro por lo general se mantendra utilizando ActionResult de t para asi poder siempre retornar el tipo de dato que yo quiero retornar y asi tener la asistencia de Visual Studio si me he equivocado.