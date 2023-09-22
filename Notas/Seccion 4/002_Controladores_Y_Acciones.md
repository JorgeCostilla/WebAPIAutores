# CONTROLADORES Y ACCIONES

La idea fundamental de tener un Web Api es que tendremos clientes que harán peticiones HTTP a nuestro Web APi. 

Estas peticiones se hacen unas URLs de nuestro dominio. A estas URLs les llamamos Rutas de nuestra Web API. Una ruta de nuestra web API es un recurso.

Un ejemplo de ruta podrias ser: https://miapi.com/autores.

https://miapi.com -> es el dominio

/autores -> es el Recurso.

Tipicamente la ruta unicamente mencionamos el recurso.

Cuando accedemos a una ruta de esta webApi tipicamente queremos que acceda a la funcion de un controlador a esta funcion le llamamos accion aunque tambien se le puede llamar (endPoint).

Una Acción (o endpoint) es una funcion de un controlador que se ejecuta en respuesta a una peticion HTTP realizada a nuestro Web API.

Un controlador es una clase que agrupa un conjunto de acciones.

## Controladores

- Nomenclatura: Nombre + Controller
- Ejemplo: AutoresController
- Los controladores normalmente se colocan en una carpeta llamada Controllers.

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
    [Route("api/autores")] // Ruta que tendra despues del dominio
    public class AutoresController:ControllerBase //Nombre del controlador hereda de ControllerBase
    {
        private readonly ApplicationDbContext context;

        public AutoresController(ApplicationDbContext context)
        {
            this.context = context;
        }

        // Esto es un endpoint
        [HttpGet] // -> Esto es un atributo
        public async Task<ActionResult<List<Autor>>> Get()
        {
            return await context.Autores.Include(x => x.Libros).ToListAsync();
        }

        // Esto es un endpoint
        [HttpGet("primero")] // api/autores/primero (ruta para distingir del metodo get general)
        public async Task<ActionResult<Autor>> PrimerAutor()
        {
            return await context.Autores.FirstOrDefaultAsync();
        }

        // Esto es un endpoint
        [HttpPost]
        public async Task<ActionResult> Post(Autor autor)
        {
            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        // Esto es un endpoint
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(Autor autor, int id)
        {
            // Validamos que sea el id del autor correcto
            if (autor.Id != id)
            {
                return BadRequest("El id del autor no coincide con el id de la URL");
            }

            // Validamos que sea el id del autor correcto
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            // Actualizamos y guardamos
            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        // Esto es un endpoint
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            // Validamos que sea el id del autor correcto
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor() {Id = id});
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
```