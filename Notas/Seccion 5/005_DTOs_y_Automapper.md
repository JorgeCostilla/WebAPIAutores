# DTOs Y AUTOMAPPER

En este video vamo a hablar acerca de DTOs y Automapper.

DTOs (Data Transfer Object o en espñaol objetos de transferencia de datos) son simplemente unas clases en las cuales vamos a utilizar para enviar datos a los clientes de nuestra clase y recibir datos de los clientes de nuestra clase.

Por ejemplo, en vez de nosotros tener este parámetro de entrada autor en este método post que tenemos aca.

```csharp
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

Nosotros deberiamos tener una clase la cual va a tener las propiedades que esperamos recibir del cliente. 

¿Que se logra con esto?

Pues que nuestra Web API pueda evolucionar sin tener que estar demasiado perocupado por afectar a los clientes del Web API.

Por ejemplo, aquí yo tengo propiedades como nombre, pero que tal si ya yo no quisiera que la clase Nombre se llame asi. Que tal si yo quiero cambiarle su nombre quisa se me pida que le cambie de Nombre a Nombres para que deba ingresar primer y segundo nombre pero ya esto afectaria a los clientes del Web API.

Si el campo ahora se llama Nombres esto va a afectar la funcionalidad de Swagger. Y aunque Swagger es autogeneradp y por lo tanto no es un problema, no podemo decir lo mismo de aplicaciones de React Angular Blazor Xamarin Flutter y ccualquier otra tecnologia cliente de nuestro web API que no es auto generada y que por lo tanto va a ser afectada pora estos cambios que se hicieron.

``` cSharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no debe de tener más de {1} caracteres")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
    }
}
```

Sin embargo si utilizamos DTOs, lo que estamos haciendo es crear algo asi como una capa que va a separar a nuestro web API de nuestros clientes y dicha capa va a contener las propiedades que los clientes deben de utilizar para poder trabajar con nuestra web API.

Asi, si internamente yo quiero actualizar mis entidades, mis bases de datos , mis clases y todo eso yo puedo hacerlo libremente siempre y cuando el DTO se mantenga constante.


Creamos una carpeta que se llama DTOs y creamos una clase que se llama AutorCreacionDTO.cs

```c#
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.DTOs
{
    public class AutorCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no debe de tener más de {1} caracteres")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
    }
}
```

Y ahora en el controller se cambia el objeto que se va a recibir.

Pero ahorita tenemos un problema en la linea context.Add(autorCreacionDTO) porque EFCore no conoce este objeto

```c#
[HttpPost]
public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO)
{
            
    var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);

    if (existeAutorConElMismoNombre)
    {
        return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");
    }

    context.Add(autorCreacionDTO);
    await context.SaveChangesAsync();
    return Ok();
}
```

¿ Que podemos hacer ?

Se tiene que hacer un mappeo automatico (Si el DTO tiene nombre y la entidad Autor tambien pues se coloca)

Para esto se necesita una libreria que se llama automapper

```cmd
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```

Una vez instalado lo necesitamos configurarlo en la clase Startup


Se debe de configurar en nuestro Startup.cs
```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers(opciones =>{
        opciones.Filters.Add(typeof(FiltroDeExcepcion));
    }).AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));
                
        // Esto se configurara completo mas adelante por ahora con eso ya podemos indicar Authenticacion
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(); 

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiAutores", Version = "v1" });
            });
    
    // Esta es la linea para configurar el automapper
    services.AddAutoMapper(typeof(Startup));
}
```


Despues se crea una carpeta de Utilidades y creamos una clase que se llama AutoMapperProfiles que debe de heredar de Profile
```c#
using AutoMapper;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Utilidades
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AutorCreacionDTO, Autor>();
        }
    }
}
```

Ahora mandamos llamar el servicio en el controlador de Autor
```c#
public AutoresController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
```

Y el Endpoint queda de la siguiente manera
```c#
[HttpPost]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO)
        {
            
            var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);

            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);

            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }
```