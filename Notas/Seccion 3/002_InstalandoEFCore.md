# Instalando Entity Framework Core

Sirve para crear una base de datos a partir del codigo de c# y no solo eso incluso si ya tuvieramos una base de datos existente podriamos conectarno con la misma atraves de una proceso database first lo cual generaria las entidades de una bd y generaria el dbContext.

## ¿Que es el dbContext?
Es el archivo donde podemos configurar EntityFramework en nuestra aplicacion, a traves de este podemos configurar el connection string (representa la bd), reglas de validacion y las tablas que vamos a usar

## ¿Como instalar EF con dotnet CLI?
1. Comando #1
```cmd
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

2. Comando #2
```cmd
dotnet add package Microsoft.EntityFrameworkCore.Design
```

## Creando el dbContext
Ya una vez teniendo instalado EF core creamos un archivo llamado ApplicationDbContext.cs

```csharp
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

        // Indicamos la creacion de una Tabla apartir del esquema de la clase Autor 
        // y la tabla tendra el nombre de Autores.
        public DbSet<Autor> Autores { get; set; }
    }
}
```


## Archivos de configuracion
Estos archivos son importantes para saber en que entorno me encuentro trabajando en estos archivos van los connection string. Si estoy trabajando localmente en modo desarrollo debo de usar el **appsettings.Development.json**

```json 
// Archivo: appsettings.Development.json con el connection string
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "defaultConnection": "Server=MSI\\SQL;Database=CursoWebApi;Persist Security Info=True;User Id=sa;Password=jorge;MultipleActiveResultSets=True;TrustServerCertificate=True"
  }
}
```

Ademas de esto agregamos en el archivo Startup.cs el dbContext en el siguiente metodo

``` csharp
 public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();
        }
```

## Creamos una migracion desde el dotnet CLI una vez realizado la conexion a la BD

Instala EF Global primero si no lo tienes instalado
```cmd
dotnet tool install --global dotnet-ef
dotnet tool list --global
```

```cmd
dotnet ef migrations add Inicial
```

Una vez ya echo la migracion se crea una carpeta con el nombre **Migrations** y ahi se colocan los archivo relacionados a estas migraciones.

Una vez revisado esto y ejecutado el comando anterior ahora toca el siguiente (para lanzar las modificaciones de las migraciones pendientes) si la base de datos no existe se crea la base de datos.

```cmd
dotnet ef database update
```