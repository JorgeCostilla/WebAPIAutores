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
    }
}