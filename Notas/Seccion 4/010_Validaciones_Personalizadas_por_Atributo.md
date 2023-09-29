# VALIDACIONES PERSONZALIDAS POR ATRIBUTO (REUTILIZABLES)

En esta clase vamos hacer una validacion personalizada pero lo vamos hacer de una forma que pueda ser reutilizable.

Por ejemplo en mi entidad Autor de ejemplo puedo notar que las propiedades Nombre y Tarjeta de Credito tienen el atributo Required que hacer que sean requeridos. Y se puede reutilizar para todas la propiedades que deseo.
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
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [NotMapped]
        public string TarjetaDeCredito { get; set; }
        [Url]
        [NotMapped]
        public string Url { get; set; }
        public List<Libro> Libros { get; set; }
    }
}
```

Entonces quiero aplicar algo similar pero personalizado

## ¿Como lo hago personalizado?

### Quiero hacer una validacion personalizada reutilizable que el string tenga que iniciar con mayuscula. Esto nos servira para nombres de personas entre otras propiedades.

-**PASO 1:** Creamos una carpeta en el proyecto que se llame Validaciones (Aqui agregaremos nuestras reglas de validacion).

-**PASO2:** Creamos una nueva clase en esta carpeta con el nombre de PrimeraLetraMayusculaAttribute.cs (Le ponemos Attribute porque va ser un atributo). Para que sea una regla de validacion utilizable como atributo *Esta clase necesita heredar de ValidationAttribute*
```C#
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Validaciones
{
    class PrimeraLetraMayusculaAttribute: ValidationAttribute
    {
        
    }
}
```

-**PASO 3:** Despues necesitamos poner el siguiente codigo y dentro de aqui ira mi logica de validacion iran dentro de este codigo.

El parametro object value sera el valor del campo (por ejemplo el campo nombre).

ValidationContext validationContext yo tengo acceso como al mismo objeto completo que tenemos aca (Autor).

```c#
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Validaciones
{
    class PrimeraLetraMayusculaAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Aquí ira mi logica de validacion
        }
    }
}
```

-**PASO 4:** En nuestro caso en esta prueba solamente queremos acceder al valor value que tenemos. Para eso colocamos e siguiente codigo pero como nosotros usaremos required para validar que lo necesita pues si el campo es null o vacio pues lo dejamos pasar y retornamos un valor satisfactorio debido a que pues la validacion que valida eso es Required.

```c#
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Validaciones
{
    class PrimeraLetraMayusculaAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            // Aquí ira mi logica de validacion
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }
        }
    }
}
```

-**PASO 5:** Ahora escribimos la logica de lo que queremos validar
```c#
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Validaciones
{
    class PrimeraLetraMayusculaAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var primerLetra = value.ToString()[0].ToString();

            if (primerLetra != primerLetra.ToUpper())
            {
                return new ValidationResult("La primera letra debe de ser mayúscula");
            }

            return ValidationResult.Success;
        }
    }
}
```

-**Paso 6:** Con esto ya puedo utilizar mi validacion en mi entidad y reutilizarla
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

Con esto ya podemos utilizar este atributo en cualquier entidad y en cualquier propiedad

***No siempre vamos a querer tener las reglas de validacion de esta manera. Existe otra manera que es la validacion por modelo***