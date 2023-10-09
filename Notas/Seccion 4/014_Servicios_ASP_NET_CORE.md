# SERVICIOS EN ASP.NET

En este video vamos a hablar del sistema de inyeccion de dependencias.

Tambien vamos a hablar de servicios y los distintos tiempos de vida de un servicio.

## Donde configuramos los servicios?

En el metodo ConfigureServices de la clase Startup.cs vamos a configurar todos los servicios.

Un servicio no es mas que la resolucion de una dependencia configurada en el sistema de inyeccion de dependencias. 

Con el sistema de inyeccion de dependencias vamos a centralizar eso que vimos en el video anterior donde teniamos que ir manualmente instanciando las dependencias de las dependencias de las dependencias en vez de hacerlo asi lo podemos configurar aqui para que resuelva o supla todas las dependencias de nuestras clases.

### Ejemplo
Aqui configuramos el servicio ISevices. Esto quiere decir que siempre que exista una dependencia de IServicio pues se le pasara una instancia del servicio a y no solo eso sino tambien las dependencias del servicio A
```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers().AddJsonOptions(x => 
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

    services.AddDbContext<ApplicationDbContext>(options => 
        options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

    services.AddTransient<IServicio, ServicioA>();

    services.AddEndpointsApiExplorer();

    services.AddSwaggerGen();
}
```

## Como hacer que sea una clase como un servicio?
Pues el codigo seria asi

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers().AddJsonOptions(x => 
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

    services.AddDbContext<ApplicationDbContext>(options => 
        options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

    // services.AddTransient<IServicio, ServicioA>();
    services.AddTransient<ServicioA>();

    services.AddEndpointsApiExplorer();

    services.AddSwaggerGen();
}
```


## Existen diferentes tipos de servicio

- AddTransient -> Agregar un transitorio (Cuando se nos solicite resolver se nos dara una instancia de servicio A) Es bueno para simples funciones sin tener que mantener data. Ejemplo la validacion de la PrimeraLetraMayuscula.  No utiliza estado

- AddSingleton -> Siempre sera la misma instancia para distintos usuarios. Nos sirve con cache con data en memoria y debe de ser uniforme asi todos los usuarios tendran la misma data compartida entre todos.

- AddScoped -> El tiempo de vida de la instancia de servicio A aumenta (Es una diferente para cada usuario). Es el alication dbContext configura el servicio como scoped la misma peticion tendra la misma instancia, para trabajar siempre con los mismo datos