# VALIDACIONES POR MODELO

En este video vamos a estudiar la reglas de validacion a nivel del modelo.

Estas validaciones son cuando la validacion es propia del modelo. Son cuando vas a combinar distintas propiedades de la entidad en este caso tiene sentido crear una regla de validacion a traves del modelo.

Antes de modificar el codigo en esta clase se ve de la siguiente manera
```c#
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
        [StringLength(maximumLength: 5, ErrorMessage = "El campo {0} no debe de tener más de {1} caracteres")]
        [PrimeraLetraMayuscula]
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

Lo primero que haremos sera heradar a esta clase de IValidatableObject e implementamos el metodo para la interface (Aqui dentro haremos las validaciones)
```c#
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.Entidades
{
    public class Autor: IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 5, ErrorMessage = "El campo {0} no debe de tener más de {1} caracteres")]
        [PrimeraLetraMayuscula]
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            
        }
    }
}
```

Agregamos la validacion de primer letra necesaita se mayuscula

**new string[] { nameof(Nombre) })** Esta parte del codigo es para agregar el campo que tuvo el error

**yield** Que hacemos con el yield bueno en el metodo de la interfaz vemos que retornamos un IEnumerable(que retornamos una coleccion de resultados de validacion) entonces para yo ir llenando esa coleccion de resultado de validacion hacemos un yield entonces cada que se ejecute ese yield se insertara en la coleccion

```c#
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.Entidades
{
    public class Autor: IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 5, ErrorMessage = "El campo {0} no debe de tener más de {1} caracteres")]
        [PrimeraLetraMayuscula]
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraLetra = Nombre[0].ToString();

                if (primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("La primer letra debe ser mayúscula",
                        new string[] { nameof(Nombre) });
                }
            }
        }
    }
}
```

Agregamos otra validacion de Menor tiene que ser menor que Mayor y comentamos la validacion personalizada de primer letra mayusucla para probar la validacion por modelo de la misma
```c#
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.Entidades
{
    public class Autor: IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 5, ErrorMessage = "El campo {0} no debe de tener más de {1} caracteres")]
        // [PrimeraLetraMayuscula]
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

        [NotMapped]
        public int Menor { get; set; }
        [NotMapped]
        public int Mayor { get; set; }
        public List<Libro> Libros { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraLetra = Nombre[0].ToString();

                if (primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("La primer letra debe ser mayúscula",
                        new string[] { nameof(Nombre) });
                }
            }

            if (Menor > Mayor)
            {
                yield return new ValidationResult("Este valor no puede ser más grande que el campo Mayor",
                    new string[] { nameof(Menor) });
            }
        }
    }
}
```


**Como dato adicional para que las validaciones por modelo entren en juego o se prueben primero tenemos que pasar todas las reglas de validacion a traves de atributos entonces estas relgas por modelo es como un segunddo nivel**

Al comentar las propiedades de prueba con sus reglas que son (Edad, TarjetaDeCredito, Url) ya entro en juego las reglas de validacion por modelo.

Este es un ejemplo de lo que mostro el resultado de errores por validacion
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-4e4a04e9d44b979a24de19cd5492df0c-70b8ece73a168770-00",
  "errors": {
    "Menor": [
      "Este valor no puede ser más grande que el campo Mayor"
    ],
    "Nombre": [
      "La primer letra debe ser mayúscula"
    ]
  }
}
```

