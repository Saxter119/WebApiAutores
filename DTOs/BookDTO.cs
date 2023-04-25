

using System.ComponentModel.DataAnnotations;
using webAPIAutores.Entidades;
using webAPIAutores.Validaciones;

namespace webAPIAutores.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }
        [CapLetter]
        [StringLength(maximumLength: 250)]
        public DateTime FechaDePublicacion { get; set; }
        public string Titulo { get; set; }
    }
}