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
    [Route("api/libros")]
    public class LibrosController: ControllerBase
    {
        private readonly ApplicationDbContext context;

        public LibrosController(ApplicationDbContext context)
        {
            this.context = context;
        }

        // [HttpGet("{id:int}")]
        // public async Task<ActionResult<Libro>> Get(int id)
        // {
        //     return await context.Libros.Include(x => x.Autor).FirstOrDefaultAsync(x => x.Id == id);
        // }

        // [HttpPost]
        // public async Task<ActionResult> Post(Libro libro)
        // {
        //     // Validamos primero que un autor exista
        //     var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);

        //     if (!existeAutor)
        //     {
        //         return BadRequest($"No existe el autor del Id: {libro.AutorId}");
        //     }

        //     context.Add(libro);
        //     await context.SaveChangesAsync();
        //     return Ok();
        // }
    }
}