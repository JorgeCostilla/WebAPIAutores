# FILTROS PERSONALIZADOS Y GLOBALES

Nos permiten reutilizar codidgo en diferentes estapas de vida de middleware.

Esto nos permitira utilizar este codgio en cualquier controller antes y despues de cualquier accion.

### Paso 1

Creacion de una nueva carpeta con el nombre de Filtros.

Creamos un archivo llamado MiFiltroDeAccion.cs y agregamos que herede de IActionFilter.

Tendremos dos metodos OnActionExecuted y OnActionExecuting estos se crean al generar la interfaz con ctrl + .

Por el momento el codigo se ve asi 

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPIAutores.Filtros
{
    public class MiFiltroDeAccion : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
```

### Paso 2

Reacomodamos los metodos OnActionExecuting lo colocamos primero porque es el que se ejecuta primero y cuando a finalizado la accion se ejecuta OnActionExecuted por eso lo pasamos en el segundo.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPIAutores.Filtros
{
    public class MiFiltroDeAccion : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            throw new NotImplementedException();
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            throw new NotImplementedException();
        }

    }
}
```

### Paso 3

Para esta prueba limpiamos los metodos y agregamos una lo siguiente para realizar pruebas como logger

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPIAutores.Filtros
{
    public class MiFiltroDeAccion : IActionFilter
    {
        public ILogger<MiFiltroDeAccion> logger { get; }

        public MiFiltroDeAccion(ILogger<MiFiltroDeAccion> logger)
        {
            this.logger = logger;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation("Antes de ejecutar la accion");
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            logger.LogInformation("Despues de ejecutar la accion");
        }

    }
}
```

### Paso 4 

Para poder usar este filtro lo necesito agregar al sistema de inyeccion de dependencias entonces lo llamare en la clase Startup.cs

El archivo de ve asi

```c#
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.Filtros;
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

            // Aqui se agrego el filtro personalizado
            services.AddTransient<MiFiltroDeAccion>();
            
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


### Paso 5

Ahora lo agregamos en el endpoint que lo quiero utilizar
```c#
[HttpGet("GUID")]
// [ResponseCache(Duration = 10)]
[ServiceFilter(typeof(MiFiltroDeAccion))]
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

Y eso es todo ya con eso puedo usar nuestra logica de nuestro filtro personalizado

### Paso 6

ahora como resultado al ejecutar la accion saldra el logInformation

```cmd
warn: Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware[3]
      Failed to determine the https port for redirect.
info: WebAPIAutores.Filtros.MiFiltroDeAccion[0]
      Antes de ejecutar la accion
info: WebAPIAutores.Filtros.MiFiltroDeAccion[0]
      Despues de ejecutar la accion
info: WebAPIAutores.Middlewares.LoguearRespuestaHTTPMiddleware[0]
      {"autoresController_Transient":"1665910e-37e3-47f4-92bd-6be6f12113a8","servicioA_Transient":"fbb0ada9-d374-46d3-991a-ff210889efdf","autoresController_Scoped":"61481d3a-9c59-4fc6-aabf-0f4a63c1a6f2","servicioA_Scoped":"61481d3a-9c59-4fc6-aabf-0f4a63c1a6f2","autoresController_Singleton":"4ef32ee8-7808-4948-9647-b491f63be8c6","servicioA_Singleton":"4ef32ee8-7808-4948-9647-b491f63be8c6"}
```



## AHORA VEREMOS EL FILTRO GLOBAL 

### En este ejemplo haremos un filtro global para atrapar todos la excepciones que no estemos atrapando en catch y esto nos permitira un mejor manejo de errores. Esto sera mas util en la seccion de azure.

## Paso 1: 

Creamos una nueva clase Llamada FiltroDeExcepcion

## Paso 2

Colocamos la logica para guardar en log el error que arroja nuestra web api

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPIAutores.Filtros
{
    public class FiltroDeExcepcion : ExceptionFilterAttribute
    {
        public ILogger<FiltroDeExcepcion> logger { get; }

        public FiltroDeExcepcion(ILogger<FiltroDeExcepcion> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}
```


### Paso 3

Ahora configuramos en Startup este filtro pero se agregara de manera diferente porque sera usada de manera globa

En esta parte del Startup se agrega el filtro de manera global
```c#
services.AddControllers(opciones =>{
    opciones.Filters.Add(typeof(FiltroDeExcepcion));
}).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
```