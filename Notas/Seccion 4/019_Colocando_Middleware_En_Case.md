# COLOCANDO EL MIDDLEWARE EN SU PROPIA CLASE

Se creo una clase Para crear el Middleware 

El ejemplo tiene nombre de LoguearRespuestaHTTPMiddleware.cs aqui creamos la logica de lo que quiero que haga el middleware

```c#
public class LoguearRespuestaHTTPMiddleware
{
    private readonly RequestDelegate siguiente;
    private readonly ILogger<LoguearRespuestaHTTPMiddleware> logger;

    public LoguearRespuestaHTTPMiddleware(RequestDelegate siguiente, 
        ILogger<LoguearRespuestaHTTPMiddleware> logger)
    {
        this.siguiente = siguiente;
        this.logger = logger;
    }

    // Invoke o InvokeAsync
    public async Task InvokeAsync(HttpContext contexto)
    {
        using (var ms = new MemoryStream())
        {
            var cuerpoOriginalRespuesta = contexto.Response.Body;
            contexto.Response.Body = ms;

            await siguiente(contexto); // Con eso le permito a la tuberia de procesos continuar

            // Despues de este await se va a ejecutar lo que los otros middleware me van a retornar.
            ms.Seek(0, SeekOrigin.Begin);
            string respuesta = new StreamReader(ms).ReadToEnd();
            ms.Seek(0, SeekOrigin.Begin);

            await ms.CopyToAsync(cuerpoOriginalRespuesta);
            contexto.Response.Body = cuerpoOriginalRespuesta;

            logger.LogInformation(respuesta);
        }
    }
}
```

Ademas agregamos una clase static (tiene que se static) para no exponer que Clase es el middleware en la clase Startuo
```c#
public static class LoguearRespuestaHTTPMiddlewareExtensions
{
    public static IApplicationBuilder UseLoguearRespuestaHTTP(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
    }
}
```

Ahora te muestro como quedo el archivo LoguearRespuestaHTTPMiddleware

```c#
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Middlewares
{

    public static class LoguearRespuestaHTTPMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoguearRespuestaHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
        }
    }

    public class LoguearRespuestaHTTPMiddleware
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<LoguearRespuestaHTTPMiddleware> logger;

        public LoguearRespuestaHTTPMiddleware(RequestDelegate siguiente, 
            ILogger<LoguearRespuestaHTTPMiddleware> logger)
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }

        // Invoke o InvokeAsync
        public async Task InvokeAsync(HttpContext contexto)
        {
            using (var ms = new MemoryStream())
            {
                var cuerpoOriginalRespuesta = contexto.Response.Body;
                contexto.Response.Body = ms;

                await siguiente(contexto); // Con eso le permito a la tuberia de procesos continuar

                // Despues de este await se va a ejecutar lo que los otros middleware me van a retornar.
                ms.Seek(0, SeekOrigin.Begin);
                string respuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(cuerpoOriginalRespuesta);
                contexto.Response.Body = cuerpoOriginalRespuesta;

                logger.LogInformation(respuesta);
            }
        }
    }
}
```


Y te muestro como quedo el archivo Startup.cs (El metodo Configure cambio)
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
```
