using webAPIAutores.DTOs;

namespace webAPIAutores.Utilidades
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDTO paginacionDTO)
        {
            return queryable
                .Skip((paginacionDTO.Pagina - 1) * paginacionDTO.RecordDePagina).Take(paginacionDTO.RecordDePagina);
        }
    }
}
