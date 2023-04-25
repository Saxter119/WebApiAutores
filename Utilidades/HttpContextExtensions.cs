using Microsoft.EntityFrameworkCore;

namespace webAPIAutores.Utilidades
{
    public static class HttpContextExtensions
    {
        public async static Task InsertarParametrosPaginacionEnCabezera<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            if(httpContext is null) { throw new ArgumentNullException(nameof(httpContext)); }

            double cantidad = await queryable.CountAsync();
            httpContext.Response.Headers.Add("cantidadTotalRegistros", cantidad.ToString());
        }
    }
}
