# INTRODUCCIÓN

Las ideas de las configuraciones es que nuestra aplicacion tendra la necesidad de consumir informacion en provedores externos, comno en un archivo de configuracion.

La idea es que es mala practica estar colocando ciertas informaciones dentro de nuestra aplicacion, ya sea por asuntos de seguridad o por asuntos de que tendríamos que recompilar nuestra aplicación cada vez que queramos cambiar ciertar configuraciones, como el servidor donde se encuentra nuestra base de datos.


# INTRODUCCION A LAS CONFIGURACIONES

Las configuraciones son datos externos de nuestra aplicacion que ayudan a nuestra aplicación a funcionar correctamente.

Ejemplo: connection string

Este ejemplo hace referencia a que no deberiamos tenerlo en el codigo fuente porque sino tendriamos que tener varios codigos fuentes por ambiente. Es mejor tener estas y otras cosas en configuraciones externas.

Para comunicarnos con estas configuraciones externas utilizamos los proveedores de configuracion.

Los proveedores de configuracion nos permiten comunicarnos con distintos tipos de fuentes externar de nuestra aplicación, ya sea que queramos comunicarnos con archivo de JSON, variables en memoria, argumentos de línea de comando, entre otros.

Debemos utilizar proveedores de configuración para poder consumir esas fuentes externas de datos.

La idea es que el framework de .NET nos permite tener un servicio a través del cualo podemos acceder a estos datos directa y uniforme. Este servicio es el IConfiguration.

Con el IConfiguration podemos acceder a los datos de configuración de nuestra aplicación. Por defecto una aplicación desde .NET Core viene preparada para trabajar con distintas fuentes de configuración.

De hecho, ya hemos utilizado configuraciones en nuestra aplicación.

Ejemplo
```json
{
  "apellido": "Costilla",
  "ConnectionStrings": {
    "defaultConnection": "Server=MSI\\SQL;Database=CursoWebApi;Persist Security Info=True;User Id=sa;Password=jorge;MultipleActiveResultSets=True;TrustServerCertificate=True"
  }
}

```

# EJEMPLO DE ICONFIGURATIONS

En un controlador vamos a crear un endpoint en el cual vamos a tomar la configuracion del apellido.

Para eso vamos a utilizar IConfiguration que ya viene configurado para usarlo. Se utiliza el de microsoft.Extensions.Configurations.

Al principio del controlador se configura.

```c#

private readonly ApplicationDbContext context;
private readonly IMapper mapper;
private readonly IConfiguration configuration;

public AutoresController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
{
    this.context = context;
    this.mapper = mapper;
    this.configuration = configuration;
}
```

Ahora en el endpoint colocamos el nombre de la configuracion la cual queremos tomar su valor.

De esta forma quedaria nuestro endpoint.

```c#
[HttpGet("configuraciones")]
public ActionResult<string> ObtenerConfiguracion()
{
    return configuration["apellido"];
}
```

Ahora vamos a aprender a extraer el valor de un connection string. 

Tenemos dos formas de hacer esto la primera es utilizar una funcion especial que esta diseñada especialmente para extraer informacion de connection strings (Esta funcion ya la hemos utilizado). 

Si vamos a la clase startup.cs y vamos hacia ConfigureServices() vamos a ver que cuando configuramos nuesta conexion a nuestro DBContext podemos observar que obtenemos el connection string a traves del servicio Configuration. El metodo especial es GetConnectionString.

Ejemplo:

```c#
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebAPIAutores.Filtros;
using WebAPIAutores.Middlewares;

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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));
        }
    }
}
```

Pero no es la unica forma tambien se puede utilizar como el ejemplo del apellido.

En el mismo endpoint podemos obtener ahora el connectionString la diferencia es que para yo entrar a un elemento dentro de otro se utiliza dos  puntos :.

```c#
[HttpGet("configuraciones")]
public ActionResult<string> ObtenerConfiguracion()
{
    return configuration["ConnectionStrings:defaultConnection"];
}
```

No estamos limitados unicamente a utilizar el appsettings.Development.json para poder trabajar con configuraciones. Podemos utilizar cualquier otro proveedor de configuraciones a traves del IConfiguration.

# USANDO EL APPSETTINGS.JSON

Nuestra API cuenta con un appsettings.json y un appsettings.development.json

La idea es que el appsettings.development.json lo vas a utilizar en tu ambiente de desarrollo. Que en nuestro caso es nuestra maquina local. Y el appsettings.json es el que se va a utilizar en tu ambiente de produccion.

De esta manera yo puedo tener diferentes connection strings entre los dos diferentes appsetings.

```c#
// Archivo appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "defaultConnection": "Data Source=url_servidor_azure;Initial Catalog=CursoWebApis;Mis credenciales"
  }
}
```


```c#
// Archivo appsettings.development.json
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
    //"defaultConnection": "Server=JCOSTILLAL;Database=CursoWebApi;Persist Security Info=True;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True"
  },
  "apellido": "Costilla"
}

```

Estos dos son diferentes archivos en el ambiente de produccion apuntara a un servidor de Azure (codigo para ejemplo) y el ambiente de desarrollo para una base de datos local

¿Como yo puedo probar y cambiar entre ambientes desde visual studio sin tener que publicar la aplicacion?
Clic derecho en WebAPIAutores -> propiedades -> Debug (Depurar) -> Abrir la interfaz de perfiles de inicio de depuración (Aqui podemos las variables de ambiente) Al cambiar la palabra a production ahora ya estamos apuntando a Produccion.


# VARIABLES DE AMBIENTE

Vamos a habalr de las variables de ambiente como un proveedor de la configuracion.

Una variable de ambiente es un valor al cual tu puedes acceder desde un ambiente especifico y lo importante de las variables de ambiente es que nos permiten acceder a estos valores sin tener dichos valores en el codigo fuente de nuestra aplicacion.

Lo primero que quier que veamos es que efectivamente las variables de ambiente son parte del sistema de configuracion por defecto de asp.net core

Al agregar una variable de ambiente podemos obtener su valor a partir de la variable de ambiente que se aca de configurar.

Las cadenas de conexion es mala practica colocarlas en el appsettings.json se deben de colocar como una variable de ambiente.