namespace webAPIAutores.DTOs
{
    public class BookDTOWithComments : BookDTO
    {
        public List<ComentarioDTO> Comentarios { get; set; }
    }
}
