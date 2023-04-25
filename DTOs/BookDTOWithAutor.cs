namespace webAPIAutores.DTOs
{
    public class BookDTOWithAutor : BookDTOWithComments
    {
        public List<AutorDTO> Autores { get; set; }

    }
}