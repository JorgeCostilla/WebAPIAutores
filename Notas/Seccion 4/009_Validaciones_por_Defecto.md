# VALIDACIONES POR DEFECTO

Ya vimos que podemos recibir valores a travez de nuestro model biding.

En este siguiente codigo vemos que tenemos un endpoint donde recibimos a travez de un model biding el parametro Autor que se envia desde el cuerpo de la peticion.

```c#
[HttpPost]
public async Task<ActionResult> Post([FromBody] Autor autor)
{
    context.Add(autor);
    await context.SaveChangesAsync();
    return Ok();
}
```

Pero en si que es un Autor.

Vemos que el autr se compone de un id de un nombre y que puede tener varios libros. 

```c#
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
        public List<Libro> Libros { get; set; }
    }
}
```

Pero yo quiero tener un conjunto de reglas que todo autor creado necesite cumplir por ejemplo.

- Que todo autor contenga su nombre. (Entonces si yo quiero agregar un autor sin nombre a mi web API yo quiero retornar un error). Una manera sencilla de hacerlo es ir al controller de Autores y asegurarse de tener arriba [ApiController] que es lo que va a permitir de una manera automatica poder retornar un 400 BadRequest al tener un error en el modelo

## Una manera de validar es crear atributos en la misma clase (Listado de estas)


### [Required] -> ya valida el campo que tenga este atributo como requerido
```c#
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        public List<Libro> Libros { get; set; }
    }
}
```

Ahora con esto obtengo un error cada que no coloque el valor de nombre un erro 400Badrequest
```
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-eadcdf8df44fd85f1c58959a62a1a06f-69b10e7d81e9a077-00",
  "errors": {
    "Nombre": [
      "The Nombre field is required."
    ]
  }
}
```

Se puede cambiar el texto de The Nombre field is required pues claro que si. 
Es de la sigueinte manera
```c#
[Required(ErrorMessage = "El campo nombre es requerido")]
public string Nombre { get; set; }

[Required(ErrorMessage = "El campo {0} es requerido")] // Es lo mismo pero el {0} sustituye el nombre de la propiedad
```


**No se esta limitado a utilizar una regla por propiedad yo puedo utilizar varias**

### [StringLength] -> supongamos que yo no quiero un nombre que sea mayor a 5 caracteres por ejemplo.

Asi quedaria de nuestra codigo

```c#
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 5, ErrorMessage = "El campo {0} no debe de tener más de {1} caracteres")] // el {1} corresponde al valor de los caracteres en este caso es 5
        public string Nombre { get; set; }
        public List<Libro> Libros { get; set; }
    }
}
```


### [Range] -> esta es otro atributo que indique el rango en el que esta permitida por ejemplo la edad del autor tiene que se mayor a 18 y menor a 120.

En este ejemplo tambien agregarmos un atributo de [NotMapped] para que no se agregue a la base de datos o que no se tenga que relacionar con la tabla correpondiente porque no existia la columna.

```c#
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 5, ErrorMessage = "El campo {0} no debe de tener más de {1} caracteres")]
        public string Nombre { get; set; }
        [Range(18, 120)]
        [NotMapped]
        public int Edad { get; set; }
        public List<Libro> Libros { get; set; }
    }
}
```

### [CreditCard] -> no valida que este activa o que si tiene fondo valida la numeración de la tarjeta que sepa que la numeracion sea valida

### [Url] -> para validar que sea una url


Para la prueba final asi queda nuestro codigo

```c#
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 5, ErrorMessage = "El campo {0} no debe de tener más de {1} caracteres")]
        public string Nombre { get; set; }
        [Range(18, 120)]
        [NotMapped]
        public int Edad { get; set; }
        [CreditCard]
        [NotMapped]
        public string TarjetaDeCredito { get; set; }
        [Url]
        [NotMapped]
        public string Url { get; set; }
        public List<Libro> Libros { get; set; }
    }
}
```

Y asi se ven los mensajes de errores.

Si pueden observar los errores se muestran dentro del campo **"errors"** y aqui mismo se pueden mostrar los distintos mensajes de errores

```
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-5f7a26d696a8810695e72b805628cad1-1f34e57a5033451f-00",
  "errors": {
    "Url": [
      "The Url field is not a valid fully-qualified http, https, or ftp URL."
    ],
    "Edad": [
      "The field Edad must be between 18 and 120."
    ],
    "Nombre": [
      "El campo Nombre no debe de tener más de 5 caracteres"
    ],
    "TarjetaDeCredito": [
      "The TarjetaDeCredito field is not a valid credit card number."
    ]
  }
}
```

Para obtener un 200 necesitamos pues correguir todos los errores

## Si quiero validar que mi nombre inicie con mayusculas ¿estas validaciones sirven? 

La respuesta corta es no aquí si pones Pepe o pepe son aceptados porque cumple con el largo del string pero pepe no deberia de ser correcto porque es un nombre los nombres inician con mayusculas.

Por el momento hemos utilizado validaciones predefinidas como si es requerido el rango stringlength etc.

Sin embargo podemos crear nuestras propias reglas de validacion
