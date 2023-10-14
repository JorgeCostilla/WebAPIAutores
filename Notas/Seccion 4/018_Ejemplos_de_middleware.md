# EJEMPLOS DE MIDDLEWARE

En la clase Startup en el metodo Configure se encuenta todos los middleware

```c#

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

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```

Aqui se encuentra esta tuberia de procesos cada uno de esos procesos app.Swagger etc son todos los middleware que te mencionaba.

Los primeros middleware (los de arriba) son los primeros que se ejecutaba. Al llegar al middleware UseEndpoint Retornara algo y ser ira retornado hasta el final y retorna la respuesta.

Los middleware son lo que emnpiezan con Use.

IsDevelopment() es simplemente una utilida que significa que si estamos en desarrollo entonces vamos a agregar unos cuantos middleware especiales en este caso es Swagger si quisiera swagger en produccion y lo sacaria del if.


## ¿Como crear nuestro propio middleware (Run)?
IApplicationBuilder podemos configurar los middleware de nuestra aplicacion.

Como yo colocare este middleware de primero significa que sera el primer middleware que se va a ejecutar. Y como yo con ese middleware voy a detener todos los demás proceso, pues este va a ser el unico que se va a ejecutar.

Para hacer yo eso de hacer un middleware que termine los procesos y que no continuen los demas middleware. Utilizamos la funcion Run con Run yo puedo crear un middleware y hacer la funcion de cortar la ejecucion de los siguiente middleware.

```c#

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.Run(async contexto => 
    {
        await contexto.Response.WriteAsync("Estoy interceptando la tubería");
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
```

Se detiene en el primer middleware porque retorna una respuesta cortando los demas procesos (no importa a que ruta te dirijas).

No es nada util es simplemente un ejemplo.

## ¿Como condiciono si fuera una ruta especifica (con map)?

```c#

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
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
```
Con map se logra hacer una difurcacion (quiere decir que en vez de tener una tuberia linea se introduce otra posible ruta, si vamos a ruta 1 se ejecuta de lo contrario se va al siguiente)

## Imagina que tenemos un requerimiento que todas las respuetas que la api devuelva lo almacenamos en un log (por seguiridad)

La idea es que como es global el log osea en cualquier endpoint pues lo hacemos con un middleware porque todos van a pasar por aqui

Mi codigo queda asi en startp.cs

```c#

using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
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

            app.Use(async (contexto, siguiente) => 
            {
                using (var ms = new MemoryStream())
                {
                    var cuerpoOriginalRespuesta = contexto.Response.Body;
                    contexto.Response.Body = ms;

                    await siguiente.Invoke(); // Con eso le permito a la tuberia de procesos continuar

                    // Despues de este await se va a ejecutar lo que los otros middleware me van a retornar.
                    ms.Seek(0, SeekOrigin.Begin);
                    string respuesta = new StreamReader(ms).ReadToEnd();
                    ms.Seek(0, SeekOrigin.Begin);

                    await ms.CopyToAsync(cuerpoOriginalRespuesta);
                    contexto.Response.Body = cuerpoOriginalRespuesta;

                    logger.LogInformation(respuesta);
                }
            });

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

Se tuvo que agregar el ilogger en program.cs

```c#
using WebAPIAutores;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

var servicioLogger = (ILogger<Startup>)app.Services.GetService(typeof(ILogger<Startup>));

startup.Configure(app, app.Environment, servicioLogger);

app.Run();

```