# REPASO DE COMO INSERTAR REGISTROS EN LA BASE DE DATOS

En este ejemplo vamos a ver este endpoint 

```c#
[HttpPost]
public async Task<ActionResult> Post([FromBody] Autor autor)
{
            
    var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);

    if (existeAutorConElMismoNombre)
    {
        return BadRequest($"Ya existe un autor con el nombre {autor.Nombre}");
    }

    context.Add(autor);
    await context.SaveChangesAsync();
    return Ok();
}
```

Vemos que en este metodo POST recibimos un Autor (Nos van a enviar un Autor y este Autor lo vamos a almacenar en nuestra base de datos)

Para esto utilizamos el ApplicationDbContext (Es la pieza central de EF Core).

Esto lo tenemos que inyectar como un servicio.

Para poder insertar un registro necesitamos dos pasos

1. Se necesita la funcion Add (Con esto indico que estoy marcando un objeto para que sea insertado en la base de datos, no se inserta de una vez)
```csharp
context.Add(autor)
```

2. Despues de que esta marcado se usa el metodo SaveCahngeAsync() (Con estos cambios seran persistidos en nuestra base de datos)
```csharp
await context.SaveChangesAsync();
```

Sin embargo, como yo te había mencionado hace ya un tiempo, no es correcto que expongamos al mundo externo nuestras entidades, lo ideal sería utilizar una clase que tenga aquellas propiedades que yo si quiera mostrarle al mundo.

Eso me va a permitir hacer que mi web API pueda evolucionar sin que nuestros clientes tengan que verse afectados.