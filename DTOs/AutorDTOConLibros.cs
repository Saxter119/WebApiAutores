namespace webAPIAutores.DTOs
{
    public class AutorDTOConLibros : AutorDTO
    {
        public List<BookDTO> Libros { get; set; }
    }
}