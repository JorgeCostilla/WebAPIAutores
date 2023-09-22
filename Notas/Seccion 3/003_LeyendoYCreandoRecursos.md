# Leyendo y Creando Recursos desde el Controlador

Usamos **async** porque vamos a usar programacion asyncrona. Con la programacion asincrona podremos trabajar de una manera eficiente con las conexiones a la base de datos.

Devolvemos un **Task ActionResult** porque es un requisito para nuestros metodos asincronos. (Es una buena practica).

Para poder utilizar el ApplicationDbContext necesito utilizar la inyeccion de dependencias.

Para crear un constructor se usar **ctor**

```c#
private readonly ApplicationDbContext context;

public AutoresController(ApplicationDbContext context)
{
    this.context = context;
}
```

Con este constructor podemos usar ApplicationDbContext desde cualquier parte de mi clase.

## ¿Cuando configuramos este ApplicationDbContext en un sistema de inyeccion de dependencias? 
Lo configuramos cuando en la clase startup dijimos AddDbContext

## ¿Como creamos un autor en la base de datos?

```c#
[HttpPost]
public async Task<ActionResult> Post(Autor autor)
{
    context.Add(autor); // Creo el autor
    await context.SaveChangesAsync(); // Guardo todo en la base de datos de manera asincrona
    return Ok(); // Retorno un ok
}
```

Algo que vamos a ver en un futuro en este curso es que no es correcto mostrar tus entidades al mundo externo sino que usamos los reconocidos DTOs. 
Por el momento es un vistaso de un webAPI.


El codigo va en esta parte asi
```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController:ControllerBase
    {
        private readonly ApplicationDbContext context;

        public AutoresController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Autor>>> Get()
        {
            return await context.Autores.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Autor autor)
        {
            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
```