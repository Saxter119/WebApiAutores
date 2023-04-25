using System.ComponentModel.DataAnnotations;

namespace webAPIAutores.DTOs
{
    public class EditarClaims
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}