using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using webAPIAutores.DTOs;
using webAPIAutores.Validaciones;

namespace webAPIAutores.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        [CapLetter]
        [StringLength(maximumLength:250)]
        [Required]
        public string Titulo { get; set; }
        public DateTime? FechaDePublicacion { get; set; }
        public List<AutorLibro> AutoresDeLibro { get; set; }
        public List<Comentario> Comentarios { get; set; }
    }
}