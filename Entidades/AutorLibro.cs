using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using webAPIAutores.Entidades;

namespace webAPIAutores.DTOs
{
    public class AutorLibro
    {
        public int AutorId { get; set; }
        public int LibroId { get; set; }
        public int Orden { get; set; }
        public Autor Autor { get; set; }
        public Libro Libro { get; set; }
    }
}

