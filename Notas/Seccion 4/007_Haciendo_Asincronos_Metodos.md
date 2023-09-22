# HACIENDO ASINCRONOS LOS METODOS DEL CONTROLADOR

En esta clase se ve como hacer nuestros metodos mas eficientes aprovechando la programacion asincrona

El siguiente es un metodo que no aprovecha este tipo de programacion

```c#
[HttpGet]
[HttpGet("listado")] // api/autores/listado
[HttpGet("/listado")] // listado
public List<Autor> Get()
{
    return context.Autores.Include(x => x.Libros).ToList();
}
```

En cambio este siguiente codigo si lo aprovecha

```c#
[HttpGet]
[HttpGet("listado")]
[HttpGet("/listado")]
public async Task<ActionResult<List<Autor>>> Get()
{
    return await context.Autores.Include(x => x.Libros).ToListAsync()
}
```

Esto ayuda en el performance.