# REGLAS DE RUTEO

Nos permiten mappear una URL o una ruta con un endpoint.

Es muy normal que el nombre de la ruta es igual al nombre del controller.

Podemos cambiar esta parte del codigo
```
[Route("api/autores")]
```
por esta otra
```
[Route("api/[controller]")]
```

Al maestro no le agrada la segunda linea a el le gusta el nombre especifico como el la primera opcion.

## ¿Se puede tener varias rutas para un mismo endpoint?

Claro solo es de agregar su propia ruta
```c#
[HttpGet]
[HttpGet("listado")]
public async Task<ActionResult<List<Autor>>> Get()
{
    return await context.Autores.Include(x => x.Libros).ToListAsync();
}
```

## ¿Se presindir de la ruta general?
Si se puede y es de la siguiente manera
```c#
[HttpGet]
[HttpGet("listado")] // api/autores/listado
[HttpGet("/listado")] // listado
public async Task<ActionResult<List<Autor>>> Get()
{
    return await context.Autores.Include(x => x.Libros).ToListAsync();
}
```