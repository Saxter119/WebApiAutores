using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using webAPIAutores.DTOs;
using webAPIAutores.Entidades;

namespace webAPIAutores
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base (options)
        {
        
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<AutorLibro>().
            HasKey(al=> new {al.AutorId, al.LibroId});
        }
        public DbSet<Autor> Autores { get; set; } 
        public DbSet<Libro> Libro {get; set;} 
        public DbSet<Comentario> Comentarios{get; set;} 
        public DbSet<AutorLibro> Autoreslibros {get; set;}
    }
}