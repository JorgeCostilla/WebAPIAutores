# RELACION DE UNO A MUCHOS

En este ejemplo vamos a crear que un comentario pertenece a un solo libro pero un libro puede tener varios comentarios.

### 1. Creamos la entidad comentario

```c#
namespace WebAPIAutores.Entidades
{
    public class Comentario
    {
        public int Id { get; set; }
        public string Contenido { get; set; }
        public int LibroId { get; set; }
        public Libro Libro { get; set; }
    }
}
```

### 2. Modificamos la entidad libro
```c#
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        [Required]
        [PrimeraLetraMayuscula]
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }
        public List<Comentario> Comentarios { get; set; } 
    }
}
```

### 3. Creamos la tabla de Comentarios en el dataContext
```c#
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

        // Indicamos la creacion de una Tabla apartir del esquema de la clase Autor y la tabla tendra el nombre 
        // de Autores.
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
    }
}
```

### 4. Luego desde el package manager console mando la migracion a mi base de datos.

Add-Migration Comentarios 

Debo de tener los paquetes necesatios como EntityFrmaework el desing y el tools

Luego el comando
Update-Database