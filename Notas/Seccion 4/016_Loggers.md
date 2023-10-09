# LOGGERS

En este video vamos a hablar acerca de logging. Con logging podemos procesar mensajes en nuestra aplicación y colocar dichos mensajes en algun lugar. Por ejemplo yo puedo colocar los mensajes del log en una consola, puedo colocar los mensajaes en una base de datos, etc, etc, etc. 

Para trabajar con logging en ASP.NET Core usamos un servicio predefinido llamado ILogger.

Ejemplo: Lo colocamos en AutoresController

```c#
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

Al servicio le pasamos la clase desde lo estoy ejecutando por ejemplo
```
ILogger<AutoresController> logger
```

Ahora en nuestra clase AutoresController ya podemos usarlo donde nosotros queramos.

En este ejemplo lo usaremos en el metodo Get de AutoresController para probar logger

```c#
[HttpGet] // api/autores
[HttpGet("listado")] // api/autores/listado
[HttpGet("/listado")] // listado
public async Task<ActionResult<List<Autor>>> Get()
{
    logger.LogInformation("Estamos obteniendo los autores");
    servicio.RealizarTarea();
    return await context.Autores.Include(x => x.Libros).ToListAsync();
}
```

Existen diferentes niveles de log en este usamos de informacion pero hay mas como critical error, entre otros.

Bueno para la prueba ejecutamos el endpoint y vamos a ver donde esta ese mensaje.

**Existen dos posibles lugares donde se encuenta este mensaje. (Esto depende de como hays configurado tu proyecto)**
- El primer lugar donde podria estar es en una consola que se ha abierto al momento de ejecutar la aplicacion (Aquie lo encontre yo)
- El segundo es OUTPUT

Se pone aqui porque por default tenemos configurado un proveedor que escribe los log en la consola.

## ¿Que es un proveedor?
Un proveedor es lo que nos permite indicar que es lo que queremos que oucrra con los mensaje que estamos mandando. Lo queremos mostrar en la consola en un archivo de texto o en una base de datos.

Lo bueno de Ilogger es que podemos tener varios proveedores configurados en el sentido de que tu puedes mandar los mensaje a distinto lugares al mismo tiempo.

To esto configurando proveedores sin tener que estar tocando, por ejemplo el codsifo que tenemos en AutoresController. 

Es decir que es una configuracion centralizadam la cual se va a replicar en toda la aplicacion.

## Tipos de mensaje de logs

Los mensajes de logs se dividen en 6 categorias (son de menor a mayor severidad trace es el de menor severidad).

- Critical (mayor severidad)
- Error
- Warining
- Information
- Debug
- Trace (menor severidad)

Entonces en el ILogger hay una logica que dice que tu vas a configurar a partir de que categoria de mensajes tu quieres mostrar, por asi decirlo imaginemos que yo quiero mostrar los mensajes de Information eso quiere decir que se van a mostrar los mensajes de information hacia arriba.

## ¿Por que esto es importante?
Porque a veces tu tienes muchos mensajes de informacion de debug y trace pero no quieres que siempre que te salgan, digamos en tu base de datos donde tu estas guardando esos mensaje.

Entonces tu puedes tener esos mensajes en toda tu aplicacion, pero configurar cuando tu quieres que realmente se procesen esas categorias de mensajes.

## ¿Donde podemos realizar esta configuracion?
Pues por defecto, esa configuracon la tenemos en el appsettings.

Tenemos dos archivos de appsettings.json

- appsettings.json
- appsettings.Development.json (Es el que usamos en el servidor o maquina de desarrollo entonces usaremos este para este ejemplo)

Hay una parte del Loggin y en LogLevel es donde configura (pero esta configuracion la podemos hacer por namespace).

Recuarda que nuestras clases se encuentran en namespace.

Este json tiene el loggin por default
```json
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
En este json tenemos que por defecto son apartir Information y que el namespace es apartir de Warning

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "WebAPIAutores":"Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "defaultConnection": "Server=MSI\\SQL;Database=CursoWebApi;Persist Security Info=True;User Id=sa;Password=jorge;MultipleActiveResultSets=True;TrustServerCertificate=True"
  }
}
```

Se puede ser mas especifico como 
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "WebAPIAutores.Controllers":"Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "defaultConnection": "Server=MSI\\SQL;Database=CursoWebApi;Persist Security Info=True;User Id=sa;Password=jorge;MultipleActiveResultSets=True;TrustServerCertificate=True"
  }
}
```