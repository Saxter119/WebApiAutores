using System.ComponentModel.DataAnnotations;

namespace webAPIAutores.DTOs
{
    public class UsserCredential
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PassWord { get; set; }
    }
}