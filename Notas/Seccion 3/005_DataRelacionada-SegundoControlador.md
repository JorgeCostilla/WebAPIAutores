# Data Relacionada - Segundo Controlador

En esta clase se estara creando la segunda entidad la cual sera libros.
En esta parte haremos una relacion de uno a muchos un Autor puede tener muchos libros. (aunque en la vida real es de muchos a muchos) Solo por simplicidad lo haremos asi

### Creamos la entidad de libro
Cuenta con:
- Id
- Titulo del libro
- AutorId 
- Autor (Es una propiedad de navegacion) -> esta nos servira para relacionar la entidad libro y la entidad autor. Con esto podre cargar desde un libro la data del Autor que escribio el libro.

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int AutorId { get; set; }
        public Autor Autor { get; set; }
    }
}
```

### Agregamos la propiedad de navegacion en la entidad Autor
Se agrego la propiedad de navegacion **Libros** 
```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<Libro> Libros { get; set; }
    }
}
```


### Agregamos el DbSet de Libros

Nosotros agregamos explicitamente el DbSet de libros para poder usar querys con esta tabla no es unicamente para crearla a la base de datos ya que por tener la propiedad de navegacion de Libros en Autores esta ya se creara implicitamente.

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.Entidades;

namespace WebAPIAutores
{
    public class ApplicationDbContext : DbContext
    {
        // Creamos el constructo
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        // Indicamos la creacion de una Tabla apartir del esquema de la clase Autor y la tabla tendra el nombre 
        // de Autores.
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Libro> Libros { get; set; }
    }
}
```

### Creamos la migracion
dotnet ef migrations add Libros
dotnet ef database update

(Ya con esto cree la migracion y la pase a la base de datos)  


### Creamos un nuevo controlador de Libros

En este controlador creamos un contructo para poder usar el ApplicationDbContext

Una vez teniendo esto creamos un metodo para obtener el primer libro que coincida con el id del libro.

Creamos otro metodo para poder crear un libro (pero primero validamos que exista el autor esto se hace con una consulta y verificamos que exista el id de este autor)

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
    [Route("api/libros")]
    public class LibrosController: ControllerBase
    {
        private readonly ApplicationDbContext context;

        public LibrosController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Libro>> Get(int id)
        {
            return await context.Libros.Include(x => x.Autor).FirstOrDefaultAsync(x => x.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult> Post(Libro libro)
        {
            // Validamos primero que un autor exista
            var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);

            if (!existeAutor)
            {
                return BadRequest($"No existe el autor del Id: {libro.AutorId}");
            }

            context.Add(libro);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
```

### Error de ciclo infinito de propiedades de navegacion
En el metodo Get agregamos el include de Autor para poder mostrar la informacion del Autor. -> Esto esta marcando un error **500** debido a que la entidad libros tiene una propiedad de navegacion hacia Autor y en la entidad de Autor tenemos una propiedad de navegacion de sus libros, entonces se hace un bucle

***Solucion***

Necesitamos ir al Startup.cs y agregar una configuracion.

De esto: **services.AddControllers()** a esto **services.AddControllers().AddJsonOptions(x => 
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);**

Entonces mi Startup.cs queda asi

```c#
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
// using System.Text.Json.Serialization;

namespace WebAPIAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(x => 
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>{
                endpoints.MapControllers();
            });
        }
    }
}
```

### ¿Como cargo los libros del autor desde el contoller del autor?

Se agrega el include
```c#
[HttpGet]
public async Task<ActionResult<List<Autor>>> Get()
{
    return await context.Autores.Include(x => x.Libros).ToListAsync();
}
```