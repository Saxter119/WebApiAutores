

using System.ComponentModel.DataAnnotations;
using webAPIAutores.Validaciones;

namespace webAPIAutores.DTOs
{
    public class BookCreationDTO
    {
        [CapLetter]
        [StringLength(maximumLength:250)]
        public string Titulo { get; set; }
        public DateTime FechaDePublicacion { get; set; }
        public List<int> AutoresIds{ get; set; }
    }
}