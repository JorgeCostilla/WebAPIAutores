# VARIABLES DE RUTA

Se conocen a las variables de ruta tambien como parametros de ruta.

Este es un ejemplo de una variable de ruta.

```c#
[HttpGet("{id}")]
public async Task<ActionResult<Autor>> Get(int id)
{
    var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

    if (autor == null)
    {
        return NotFound();
    }
    return autor;
}
```

Para agregar una restriccion al parametro o a la variable de ruta se hace de la siguiente manera.
```c#
[HttpGet("{id:int}")]
public async Task<ActionResult<Autor>> Get(int id)
{
    var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

    if (autor == null)
    {
        return NotFound();
    }
    return autor;
}
```


No existe una restriccion para string ese solol colocas el nombre del parametro
```c#
[HttpGet("{nombre}")]
public async Task<ActionResult<Autor>> Get(string nombre)
{
    var autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));

    if (autor == null)
    {
        return NotFound();
    }
    return autor;
}
```

No se esta limitado a usar solo un parametro se puede usar la cantidad de parametros que usted quiera o requiera. El signo de iterrogacion es para que sea un valor opcional

```c#
[HttpGet("{id:int}/{param2}")]
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

Acepta nulos el param2
```c#
[HttpGet("{id:int}/{param2?}")]
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

Valor por defecto
```c#
[HttpGet("{id:int}/{param2=persona}")]
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