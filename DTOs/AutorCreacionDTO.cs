using System.ComponentModel.DataAnnotations;
using webAPIAutores.Validaciones;


namespace webAPIAutores.DTOs
{
    public class AutorCreacionDTO
    {
        [CapLetter]
        [Required(ErrorMessage = "Ta vacío el campo '{0}'")]
        [StringLength(maximumLength: 120, MinimumLength = 3, ErrorMessage = "El campo '{0}' debe estár entre {2} y {1} carácteres")]
        public string Nombre { get; set; }
    }
}