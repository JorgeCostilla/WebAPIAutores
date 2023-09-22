# CREANDO EL WEB API CON VS CODE

1. Crear una carpeta
2. Dentro de esa carpeta abrir el cmd o el powrshell
3. Escribimos el comando **dotnet new --list** (nos mostrara todas las plantillas que podemos utilizar) lo mas actualizado es usar **dotnet new list**.
4. Escribimos el comando **dotnet new webapi -o Nombre**
5. Abrimos la carpeta que se creo en VSCode 


# CORRIENDO EL WEB API CON VS CODE Y EL DOTNET CLI

1. Para poder correrlo tengo que hacerlo desde una terminal.
2. Colocar el comando **dotnet run**
3. Se enlistas dos urls uno con http y otro con https
4. Dentro de este podemos ingresar a /swagger/index.html (tambien conocido como open API) es una pagina que me permite visualizar las rutas de mi webAPI.
5. Estas rutas hacen referencia a un controlador.
6. Y esa clase tiene metodos que se conocen como endpoint.


# COMO DEBUGGEAR EN VS CODE Y VER LINEA POR LINEA EN TU WEB API

1. Colocar un breakpoint cuando llegue a este el programa se detendra y poder revisar linea por linea.
2. Abrir la terminal que ya esta usando y parar la ejecucion.
3. Ir a Run & Debug y dar en start debuggin o f5
4. Se ejecuta en como debugger f5 para que continue el programa f10 se paraliza en la siguiente linea de codigo.
5. F11 o step into es si vas a ejecutar una funcion entres a esa funcion



# CREAMOS EL ARCHIVO STARTUP.CS 

Este archivo antes te daban la plantilla desde .net6 ya no aqui se muestra como se trabajara esta clase en este curso.

El archivo Program.cs queda de la siguiente manera

```c#
using WebAPIAutores;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

app.Run();

```

El archivo Startup.cs quedo asi

```c#
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
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

# DESACTIVANDO LOS TIPOS DE REFERENCIA NO NULOS

En el archivo WebAPIAutores.csproj en la etiqueta **Nullable** colocar disable en lugar de enable.