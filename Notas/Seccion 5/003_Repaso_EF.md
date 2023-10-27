# REPASO A ENTITY FRAMEWORK CORE

EntityFramework Core es un ORM esto quiere decir que es un mapeador de objetos relacionados, esto quiere decir que de clases de c# vamos a poder interactuar con una base de datos, esta base de datos puede sql server sql lite mysql , en nuestro caso vamos a trabajar con SQL Server.

## ¿Como puedo trabajar con otras BD?

Se necesita instalar el NuGet -> Microsoft.EntityFrameworkCore.SqlServer (Es el proveedor para configurar EF con SqlServer)
Se necesita instalar el NuGet -> Microsoft.EntityFrameworkCore.Sqlite (Es el proveedor para configurar EF con Sqlite)

Toda seria igual (la manera de trabajar los querys etc) lo unico que cambiaria los querys seria para sqlLite

Ademas en Visual Studio instalamos otro paquete que es Microsoft.EntityFrameworkCore.Tools

Pero si trabajas con dotnet CLI en VsCode -> Microsoft.EntityFrameworkCore.Relational


## El siguiente paso que dimos fue trabajar con el ApplicationDbContext.cs

No es mas que una clase que hereda de dbcontext, el nombre de ApplicationDbContext es inventado tu le puedes poner el nombre que deseas pero al maestro le gusta ponerle ese nombre. 

Esta clase hereda de DbConext. Esta es la clase central de Entity Framework Core y a traves de ella es que configuramos las tablas de nuestra base de datos. 

En nuestro caso estamos utilizando codigo primero, donde apartir de codigo c# podemos generar una base de datos.

Otra tecnica sería darle la vuelta a esto y utilizar base de datos primero donde empezamos con una base de datos y apartir de ella generamos el codigo de c#. (Esto es algo que no veremos en este curso es algo que se trata en el curso de Entity Framewrok, pero queria mencionarlo para que supieras)

## Para configurar las tablas utilizamos las propiedad DbSet

```c#
// Aqui creamos una tabla con el nombre Autores con el modelo Autor
public DbSet<Autor> Autores { get; set;}
```

```c#
// Este es el modelo Autor
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

Una vez ya configurado esto, lo que haciamos era ir a un proveedor de configuracion para colocar nuestro conexion string (Indica a que base de datos nos vamos a conectar)

## Conexion String

Dado que en produccion y en desarrollo debemos tener diferentes bases de datos existen don appsetings uno es de Development y ahi configuramos el conexion string

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "WebAPIAutores":"Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "defaultConnection": "Server=MSI\\SQL;Database=CursoWebApi;Persist Security Info=True;User Id=sa;Password=jorge;MultipleActiveResultSets=True;TrustServerCertificate=True"
  }
}
```

En produccion debemos tener otro conexion string y esta base de datos se va a ubicar en Azure, pero eso lo vamos a ver más adelante


Despues de esto en la clase Startup.cs configuramos el conexion string 

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

    services.AddSwaggerGen();
}
```

## Agregamos las migraciones

Lo que nosotros hacemos con una migracion es que representamos el codigo, los cambios que vamos a ver en nuestra base de datos.

Ahora en la terminal agregamos los comandos para agregar la migracion.

```cmd
dotnet ef migrations add Inicial
```

Esto crea una migracion y genera en codigo los cambios que van a ocurrir en la base de datos.


Aqui vemos que se crea una tabla llamada Autores con columnas (Id int Identity (es identity porque la convencion de EF es que el Id es PK identity)) y (Nombre nvarchar (120))

Tambien tenemos una tabla que se llamara libros
```c#
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIAutores.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Autores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Autores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Libros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libros", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Autores");

            migrationBuilder.DropTable(
                name: "Libros");
        }
    }
}

```

Ahora esta migracion se debera de generar los cambios con el comando 

```cmd
dotnet ef database update
```

Ahora vemos que la bd se creo con las tablas que mencionamos y una tabla que se llama EFMigrationsHistory que usa EF para saber que migraciones ya fueron aplicadas la puedes ignorar si gustas.