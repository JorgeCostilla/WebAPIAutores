# INICIANDO EL DESARROLLO DEL WEB API

Limpiar la Web API es lo primero que hay que hacer, por defecto se crean un controller ese lo vamos a eliminar (WeatherForecast.cs)

En esta curso vamos crear una api de autores y libros.

## Crear controller de Autores
En la carpeta de Controllers creamos el controlador AutoresController.cs Por defecto estas clases llevan la palabra Controller.

Para que esto sea un controlador necesitamos heredar de una clase base **ControllerBase** y necesitamos traer el namespace **using Microsoft.AspNetCore.Mvc;**.

Decorar el controlador con el atributo **[ApiController]** para poder hacer validaciones

La parte del **Route** es la ruta que al llegar ahi es el controlador que manejara dicha peticion.


```csharp 
// Este es el codigo base de un controller
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController:ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Autor>> Get()
        {
            return new List<Autor>() {
                new Autor{ Id = 1, Nombre = "Felipe" },
                new Autor{ Id = 2, Nombre = "Claudia" }
            };
        }
    }
}
```

## Crear una carpeta que se llame Entidades 
Se llaman entidades porque van a corresponder con una tabla en una base de datos (Se creara en el siguiente video)

El siguiente codigo muestra la clase de autores

Con la palabra prop + doble tab se crea **public int MyProperty { get; set; }**

Tiene dos propiedades una que es el Id y otra el Nombre
```csharp
// Clase de Autor
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
```