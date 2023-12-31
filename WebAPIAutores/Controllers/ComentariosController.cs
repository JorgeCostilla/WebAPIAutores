using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class ComentariosController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ComentariosController(ApplicationDbContext context, IMapper mapper) 
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> Get (int libroId)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDb => libroDb.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var comentario = await context.Comentarios
                .Where(comentarioDB => comentarioDB.LibroId == libroId).ToListAsync();

            return mapper.Map<List<ComentarioDTO>>(comentario);
        }

        [HttpPost]
        public async Task<ActionResult> Post (int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDb => libroDb.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = libroId;
            context.Add(comentario);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
