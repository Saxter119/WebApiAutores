using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using webAPIAutores.DTOs;
using webAPIAutores.Servicios;

namespace webAPIAutores.Utilidades
{
    public class HATEOASAutorFilterAttribute : HATEOASFiltroAttribute
    {
        private readonly GestionadorDeEnlaces gestionadorDeEnlaces;

        public HATEOASAutorFilterAttribute(GestionadorDeEnlaces gestionadorDeEnlaces)
        {
            this.gestionadorDeEnlaces = gestionadorDeEnlaces;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var debeIncluir = DebeIncluirHATEOAS(context);

            if (!debeIncluir)
            {
                await next();
                return;
            }

            var result = context.Result as ObjectResult;

            var autorDTO = result.Value as AutorDTO;

            if (autorDTO == null)
            {
                var autoresDTO = result.Value as List<AutorDTO> ?? throw new ArgumentException("Se esperaba una instancia de tipo AutorDTO o List<AutorDTO>");
                
                    autoresDTO.ForEach(async autorDTO => await gestionadorDeEnlaces.ObtenerEnlaces(autorDTO));

                result.Value = autoresDTO; 

                    await next();
                
            }
            else
            {
            await gestionadorDeEnlaces.ObtenerEnlaces(autorDTO);
            }
            await next();
        }
    }
}
