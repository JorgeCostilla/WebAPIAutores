# EJEMPLOS DE TIEMPO DE VIDA DE LOS SERVICIOS

Ya dijimos que AddTransient o transitorio es que siempe se te va a dar una nueva instancia de la clase. Scoped es cuando te van a dar la misma instancia en el mismo contexto HTTP y que Singleton siempre se te va dar la misma instancia, incluso en distintas peticiones http independientemente que sean usuarios distintos.

Ejemplo: Se van a crear 3 clases en el archivo IServicio.cs

```c#
public class ServicioTrasient 
{
    // Esto lo que esta haciendo es creando un string aleatorio
    public Guid Guid = Guid.NewGuid();
}
public class ServicioScoped
{
    // Esto lo que esta haciendo es creando un string aleatorio
    public Guid Guid = Guid.NewGuid();
}
public class ServicioSingleton 
{
    // Esto lo que esta haciendo es creando un string aleatorio
    public Guid Guid = Guid.NewGuid();
}
```

Despues de crear las tres diferentes clases la vamos a configurar como servicio en el archivo Startup.cs

El codigo se ve de la siguiente manera

```c#
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
```

Ahora en AutoresController.cs, crearemos una nuevo metodo una nueva accion que sera la siguiente. Pero antes del metodo configuramos las dependencias de la clase que seran los servicios recien creados.

```c#
private readonly ApplicationDbContext context;
private readonly IServicio servicio;
private readonly ServicioTrasient servicioTrasient;
private readonly ServicioScoped servicioScoped;
private readonly ServicioSingleton servicioSingleton;

public AutoresController(ApplicationDbContext context, IServicio servicio,
    ServicioTrasient servicioTrasient, ServicioScoped servicioScoped, 
    ServicioSingleton servicioSingleton)
{
    this.context = context;
    this.servicio = servicio;
    this.servicioTrasient = servicioTrasient;
    this.servicioScoped = servicioScoped;
    this.servicioSingleton = servicioSingleton;
}
```

```c#
[HttpGet("GUI")]
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

Ademas agregamos como referencias a la clase ServicioA los servicios transient scoped y singleton
```c#
namespace WebAPIAutores.Servicios
{
    public interface IServicio
    {
        Guid ObtenerScoped();
        Guid ObtenerSingleton();
        Guid ObtenerTransient();
        void RealizarTarea();
    }

    public class ServicioA : IServicio
    {
        private readonly ILogger<ServicioA> logger;
        private readonly ServicioTrasient servicioTrasient;
        private readonly ServicioScoped servicioScoped;
        private readonly ServicioSingleton servicioSingleton;

        public ServicioA(ILogger<ServicioA> logger, ServicioTrasient servicioTrasient,
            ServicioScoped servicioScoped, ServicioSingleton servicioSingleton)
        {
            this.logger = logger;
            this.servicioTrasient = servicioTrasient;
            this.servicioScoped = servicioScoped;
            this.servicioSingleton = servicioSingleton;
        }

        public Guid ObtenerTransient() { return servicioTrasient.Guid; }
        public Guid ObtenerScoped() { return servicioScoped.Guid; }
        public Guid ObtenerSingleton() { return servicioSingleton.Guid; }

        public void RealizarTarea()
        {
        }
    }

    public class ServicioB : IServicio
    {
        public Guid ObtenerScoped()
        {
            throw new NotImplementedException();
        }

        public Guid ObtenerSingleton()
        {
            throw new NotImplementedException();
        }

        public Guid ObtenerTransient()
        {
            throw new NotImplementedException();
        }

        public void RealizarTarea()
        {
        }
    }

    public class ServicioTrasient 
    {
        // Esto lo que esta haciendo es creando un string aleatorio
        public Guid Guid = Guid.NewGuid();
    }
    public class ServicioScoped
    {
        // Esto lo que esta haciendo es creando un string aleatorio
        public Guid Guid = Guid.NewGuid();
    }
    public class ServicioSingleton 
    {
        // Esto lo que esta haciendo es creando un string aleatorio
        public Guid Guid = Guid.NewGuid();
    }
}
```


## PRUEBAS
En estas pruebas estamos ejecutando desde una clase y desde un servicio los tres diferentes tipos de Servicos

Estos son los resultados:
```json
{
  "autoresController_Transient": "e5f42495-5d2e-44ad-b1ac-aa6c75fce59a",
  "servicioA_Transient": "75fe4f36-52c7-4d3d-adb7-32ea3d68b6d7",
  "autoresController_Scoped": "43361a99-a01e-4634-9a3e-552b2535cfd4",
  "servicioA_Scoped": "43361a99-a01e-4634-9a3e-552b2535cfd4",
  "autoresController_Singleton": "b37da63f-a554-4a4f-b399-4814b00e572c",
  "servicioA_Singleton": "b37da63f-a554-4a4f-b399-4814b00e572c"
}
```
Notamos a simple vista que el Trasiente siempre sera diferente el uno del otro
y en la primera ejecucion el scoped y el singleton son iguales. Ahora si volvemos a ejecutar el singleton se mantuvo el mismo resultado pero el scoped cambio

```json
{
  "autoresController_Transient": "4530a782-bc9f-4c5a-be9c-ebbf5532ce29",
  "servicioA_Transient": "c5132bf0-1bd8-4edb-ac19-0940cc790076",
  "autoresController_Scoped": "871e3b65-7f4b-47d7-b651-cb741472f8b3",
  "servicioA_Scoped": "871e3b65-7f4b-47d7-b651-cb741472f8b3",
  "autoresController_Singleton": "b37da63f-a554-4a4f-b399-4814b00e572c",
  "servicioA_Singleton": "b37da63f-a554-4a4f-b399-4814b00e572c"
}
```
El scoped si cambia aunque es el mismo dentro del mismo context http pero entre diferentes contexto HTTP si cambia 