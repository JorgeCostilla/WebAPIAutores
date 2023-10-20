# INTRODUCCION DE FILTROS

Los filtros nos ayudan a correr codigo en determinados momentos del ciclo de vida del procesamiento de una peticion HTTP en lo que respecta al middleware relacionado con nuestros controladores.

Hasta ahora nos hemos enfocado en ejecutar una accion de un controlador tenemos la opcion de ejecutar codigo utilizando filtros.

Los filtros son útiles cuando tenemos la necesidad de ejecutar una lógica en varias acciones de varios controladores y queremos evitar tener que repetir codigo.

Uno de los filtros mas utilizados es el filtro de autorizacion(Este filtro nos permite bloquear acceso cuando no esta logeado). Si te fijas esta logica no es especifica de una sola accion, sino que multiples acciones de multiples controladores se pueden beneficiar de poder indicar que unicamente usuarios autenticados puedan realizar una accion determinada.

Cuando hablamos de autenticarse nos referimos a saber quien es con usuario y password.

## Tipos de filtros
- **Filtros de Autorizacion**: Son los que determinan si un usuario puede consumir una accion determinada. (Veremos un ejemplo de autorizacion es este mismo video).

- **Filtros de recursos** : Estos se ejecutan despues de la etapa de autorizacion. Podemos utilizarlos para validaciones generales o para implementar una capa de cache. La idea es que esos filtros ademas pueden detener la tuberia de filtros de tal modo que ni los demas filtros ni la accion del controlador se ejecueten.

- **Filtros de accion** : Los filtros de accion se ejecutan justo antes y justo despues de la ejecucion de una accion. Se puede utilizar para manipular los argumentos enviados a una accion o la informacion retornada por los mismo.

- **Filtros de Excepcion** : Se ejecutan cuando hubo una excepcion no atrapada en un try catch durante la ejecucion de una accion, un filtro de accion durante la creacion del controlador y durante el binding de modelo.

- **Filtros de resultado** : Se ejecutan antes y despues de la ejecucion de un actionResult


## Maneras tipicas de aplicar un filtro

- A nivel de acción
- A nivel de controlador
- A nivel de todo web api


## Ejemplos 

### Ejemplo de cache

Agregamos dos lineas en la clase Startup.cs
```c#
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.Middlewares;
using WebAPIAutores.Servicios;
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

            services.AddTransient<IServicio, ServicioA>();
            // services.AddTransient<ServicioA>();

            // Configuramos las clases de ejemplo como servicios
            services.AddTransient<ServicioTrasient>();
            services.AddScoped<ServicioScoped>();
            services.AddSingleton<ServicioSingleton>();

            services.AddResponseCaching(); // Se agrego para utilizar los servicios de la cache

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseLoguearRespuestaHTTP();

            app.Map("/ruta1", app =>
            {
                app.Run(async contexto =>
                {
                    await contexto.Response.WriteAsync("Estoy interceptando la tubería");
                });
            });

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseResponseCaching(); // Se agrego la tuberia de cache

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
```

Ahora en en controlador de autores (AutoresController.cs) en el endpoint GUID agregamos un atributo ResponseCache

Este atributo lo que hace es almacenar en la memoria cache la respuesta. Entonces el primer llamdado no  hay cache entonces lanza una respues el segundo llamado si es antes de los 10 segundos entoces toma el valor de la cache sino vuelve a tomar otro valor.

Es util para el tiempo de rendimiento 
```c#
[HttpGet("GUID")]
[ResponseCache(Duration = 10)]
public ActionResult ObtenerGuids()
{
    return Ok(new {
        AutoresController_Transient = servicioTrasient.Guid,
        ServicioA_Transient = servicio.ObtenerTransient(),
        AutoresController_Scoped = servicioScoped.Guid,
        ServicioA_Scoped = servicio.ObtenerScoped(),
        AutoresController_Singleton = servicioSingleton.Guid,
        ServicioA_Singleton = servicio.ObtenerSingleton(),
    });
}
```

Este filtro de cache pues se puede reutilizar en cualquier endpoint.


### Ejemplo de Autorizacion
Nos sirve para que unos recursos significa que solo usuarios o con cierto perfil pueden acceder al endpoint.

Ejemplo:

Necesitamos instalar el siguiente paquete con el comando CLI
```cmd
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

Ahora si en la clase Startup.cs agregamos el servicio y el middleware de Authentication

```c#
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.Middlewares;
using WebAPIAutores.Servicios;
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

            services.AddTransient<IServicio, ServicioA>();
            // services.AddTransient<ServicioA>();

            // Configuramos las clases de ejemplo como servicios
            services.AddTransient<ServicioTrasient>();
            services.AddScoped<ServicioScoped>();
            services.AddSingleton<ServicioSingleton>();

            services.AddResponseCaching(); // Se agrego para utilizar los servicios de la cache

            // Esto se configurara completo mas adelante por ahora con eso ya podemos indicar Authenticacion
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(); 

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseLoguearRespuestaHTTP();

            app.Map("/ruta1", app =>
            {
                app.Run(async contexto =>
                {
                    await contexto.Response.WriteAsync("Estoy interceptando la tubería");
                });
            });

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseResponseCaching(); // Se agrego la tuberia de cache

            app.UseAuthorization(); // Nos aseguramos tener esto antes de UseEndpoint

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
```

Y probamos en un metodo este atributo (por el momento nunca podremos usarlo porque no tenemos sistema de usarios)

```c#
[HttpGet] // api/autores
[HttpGet("listado")] // api/autores/listado
[HttpGet("/listado")] // listado
[Authorize]
public async Task<ActionResult<List<Autor>>> Get()
{
    logger.LogInformation("Estamos obteniendo los autores");
    logger.LogWarning("Este es un mensaje de prueba");
    servicio.RealizarTarea();
    return await context.Autores.Include(x => x.Libros).ToListAsync();
}
```

Con esto en este metodo ahora recibiremos un 401 de no autorizado

Esto esta puesto unicamente en un endpoint para bloquear todo el controlador se colocaria en el incio del controlador

```c#
[ApiController]
[Route("api/autores")]
[Authorize]
public class AutoresController:ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IServicio servicio;
    private readonly ServicioTrasient servicioTrasient;
    private readonly ServicioScoped servicioScoped;
    private readonly ServicioSingleton servicioSingleton;
    private readonly ILogger<AutoresController> logger;

    public AutoresController(ApplicationDbContext context, IServicio servicio,
        ServicioTrasient servicioTrasient, ServicioScoped servicioScoped, 
        ServicioSingleton servicioSingleton, ILogger<AutoresController> logger)
    {
        this.context = context;
        this.servicio = servicio;
        this.servicioTrasient = servicioTrasient;
        this.servicioScoped = servicioScoped;
        this.servicioSingleton = servicioSingleton;
        this.logger = logger;
    }

```