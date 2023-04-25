using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace webAPIAutores.Utilidades
{
    public class HATEOASFiltroAttribute : ResultFilterAttribute
    {
        protected bool DebeIncluirHATEOAS(ResultExecutingContext context)
        {
            var result = context.Result as ObjectResult;

            if (!EsRespuestaExitosa(result))
            {
                return false;
            }

            var cabecera = context.HttpContext.Request.Headers["includeHateoas"];

            if(cabecera.Count == 0)
            {
                return false;
            }

            var valor = cabecera[0];

            if (!valor.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;

        }

        private bool EsRespuestaExitosa(ObjectResult result)
        {
            if(result == null || result.Value == null)
            {
                return false;
            }

            if (result.StatusCode.HasValue && !result.StatusCode.Value.ToString().StartsWith("2"))
            {
                return false;
            }

            return true;
        }
    }
}
