namespace webAPIAutores.DTOs
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1;

        private int recordDePagina { get; set; } = 10;

        readonly int cantidadMaximaPorPagina = 50;

        public int RecordDePagina { get { return recordDePagina; } 
            set { recordDePagina = value > cantidadMaximaPorPagina ? cantidadMaximaPorPagina : value; } }

        
    }
}
