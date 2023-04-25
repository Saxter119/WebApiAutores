
using System.ComponentModel.DataAnnotations;
using webAPIAutores.Validaciones;

namespace webAPIAutores.DTOs
{
    public class AutorDTO : Recurso
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}