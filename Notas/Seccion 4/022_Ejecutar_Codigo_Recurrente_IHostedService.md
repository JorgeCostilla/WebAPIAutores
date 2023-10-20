# EJECUTAR CODIGO RECURRENTE CON IHOSTEDSERVICE

Existen diferentes maneras como con servicios externos de azure pero aqui se explicara con IHostedService.

Se ejecutara al inicio y al final del tiempo de vida de la webAPI (Cuando se inicia y cuando se apaga la API se ejecuta)

Creamos una clase en el folder se Service con el nombre de EscribirEnArchivo

Lo que hara este servicio sera escribir en un archivo cada 5 segundos

De manera rapida creamos la clase se vera de la siguiente forma 
```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Servicios
{
    public class EscribirEnArchivo : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
```

Heredamos de IHostedService y se implementa la interfaz con estos dos metodos.

Hay que señalar que no siempre se va a ejecutar el metodo de StopAsync, hay situaciones excepcionales que no les de ni el tiempo de ejecutarse como:

- Si tu aplicacion se detiene de manera repentina por un error.


Ahora creamos un constructor con IWebHostEnvironment que nos permitira acceder al ambiente en el cual yo me encuentro.

Ademas creamos una variable con el nombre del archivo

Ahora creamos un metodo auxiliar para escribir en el archivo

Tenemos el siguiente codigo en la clase asi se vera

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Servicios
{
    public class EscribirEnArchivo : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string nombreArchivo = "Archivo 1.txt";

        public EscribirEnArchivo (IWebHostEnvironment env)
        {
            this.env = env;
        }
        
        // Cuando se inciliza escribe en el archivo proceso iniciado
        // Ademas return de que la tarea finalzo
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Escribir("Proceso iniciado");
            return Task.CompletedTask;
        }

        // Cuando se inciliza escribe en el archivo proceso finalizado
        // Ademas return de que la tarea finalzo
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Escribir("Proceso finalizado");
            return Task.CompletedTask;
        }

        // Metodo aux en el cual escribe en un archivo de texto el mensaje
        private void Escribir(string mensaje)
        {
            var ruta = $@"{env.ContentRootPath}\wwwroot\{nombreArchivo}";
            using (StreamWriter writer = new StreamWriter(ruta, append: true))
            {
                writer.WriteLine(mensaje);
            }
        }
    }
}
```


Vamos a configurar este servicio en la clase de Startup.cs

Asi quedara este archivo

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
            services.AddControllers(opciones =>{
                opciones.Filters.Add(typeof(FiltroDeExcepcion));
            }).AddJsonOptions(x =>
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

            // Aqui agregamos el servicio de Escribir en un archivo
            services.AddHostedService<EscribirEnArchivo>();
            
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


Si inicializamos encontraremosun error como el siguiente:

```cmd
         at Microsoft.AspNetCore.ResponseCaching.ResponseCachingMiddleware.Invoke(HttpContext httpContext)
PS C:\Users\costi\Documents\004_Cursos_Udemy\001_Curso_API_NetCore6\Web API\WebAPIAutores> dotnet run 
Compilando...
Unhandled exception. System.IO.DirectoryNotFoundException: Could not find a part of the path 'C:\Users\costi\Documents\004_Cursos_Udemy\001_Curso_API_NetCore6\Web API\WebAPIAutores\wwwroot\Archivo 1.txt'.
   at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
   at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.Strategies.OSFileStreamStrategy..ctor(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.Strategies.FileStreamHelpers.ChooseStrategyCore(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
   at System.IO.StreamWriter.ValidateArgsAndOpenPath(String path, Boolean append, Encoding encoding, Int32 bufferSize)      
   at System.IO.StreamWriter..ctor(String path, Boolean append)
   at WebAPIAutores.Servicios.EscribirEnArchivo.Escribir(String mensaje) in C:\Users\costi\Documents\004_Cursos_Udemy\001_Curso_API_NetCore6\Web API\WebAPIAutores\Servicios\EscribirEnArchivo.cs:line 38
   at WebAPIAutores.Servicios.EscribirEnArchivo.StartAsync(CancellationToken cancellationToken) in C:\Users\costi\Documents\004_Cursos_Udemy\001_Curso_API_NetCore6\Web API\WebAPIAutores\Servicios\EscribirEnArchivo.cs:line 22
   at Microsoft.Extensions.Hosting.Internal.Host.StartAsync(CancellationToken cancellationToken)
   at Microsoft.Extensions.Hosting.HostingAbstractionsHostExtensions.RunAsync(IHost host, CancellationToken token)
   at Microsoft.Extensions.Hosting.HostingAbstractionsHostExtensions.RunAsync(IHost host, CancellationToken token)
   at Microsoft.Extensions.Hosting.HostingAbstractionsHostExtensions.Run(IHost host)
   at Program.<Main>$(String[] args) in C:\Users\costi\Documents\004_Cursos_Udemy\001_Curso_API_NetCore6\Web API\WebAPIAutores\Program.cs:line 15
```

Este error nos dice que practicamente no existe una ruta para ese archivo. Entonces creamos un nuevo folder con el nombre wwwroot (Que es una ruta especial en asp.net core para archivos estaticos aqui lo usare para almacenar mis archivos de texto) Ahora al correr la webAPI no tendremos ningun error.

Y al iniciar la API se crea un archivo en wwwroot con el nombre que indicamos y el texto que indicamos y al finalizar la api se actualiza con el texto que indicamos.

##  Ya validamos que existe el archivo ahora modificaremos el servicio para que sea un archivo recurrente.

Asi queda la clase del servicio y cada 5 segundo se esta ejecutando y se actualiza el archivo de texto. ( Esto es util cuando quieres hacer una funcionalidad recurrrente en tu aplicacion )

```c# 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Servicios
{
    public class EscribirEnArchivo : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string nombreArchivo = "Archivo 1.txt";
        private Timer timer;

        public EscribirEnArchivo (IWebHostEnvironment env)
        {
            this.env = env;
        }
        
        // Cuando se inciliza escribe en el archivo proceso iniciado
        // Ademas return de que la tarea finalzo
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Iniciamos el timer en 0 y se ejecutara cada 5 segundos
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            Escribir("Proceso iniciado");
            return Task.CompletedTask;
        }

        // Cuando se inciliza escribe en el archivo proceso finalizado
        // Ademas return de que la tarea finalzo
        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose(); // Detenemos el timer
            Escribir("Proceso finalizado");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Escribir("Proceso en ejecucion: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }

        // Metodo aux en el cual escribe en un archivo de texto el mensaje
        private void Escribir(string mensaje)
        {
            var ruta = $@"{env.ContentRootPath}\wwwroot\{nombreArchivo}";
            using (StreamWriter writer = new StreamWriter(ruta, append: true))
            {
                writer.WriteLine(mensaje);
            }
        }
    }
}
```