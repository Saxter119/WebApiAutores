using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using webAPIAutores.DTOs;
using webAPIAutores.Utilidades;

namespace webAPIAutores.Servicios
{
    public class GestionadorDeEnlaces
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor contextAccessor;

        public GestionadorDeEnlaces(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor, 
            IActionContextAccessor contextAccessor)
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.contextAccessor = contextAccessor;
        }

        private IUrlHelper ConstruirURLHelper()
        {
            var factoria = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();

            return factoria.GetUrlHelper(contextAccessor.ActionContext);
        }

        public async Task<bool> EsAdmin()
        {
            var httpContext = httpContextAccessor.HttpContext;

            var esAdmin = await authorizationService.AuthorizeAsync(httpContext.User, "EsAdminNoBulto");

            return esAdmin.Succeeded;
        }

        public async Task ObtenerEnlaces(AutorDTO autorDTO)
        {
            var Url = ConstruirURLHelper();

            autorDTO.Enlaces.Add(new DatoHATEOAS(Url.Link($"obtenerAutoresv1", new { id = autorDTO.Id }), descripcion: "self", "GET"));

            bool esAdmin = await EsAdmin();

            if (esAdmin)
            {
                autorDTO.Enlaces.Add(new DatoHATEOAS(Url.Link("actualizarAutorv1", new { id = autorDTO.Id }), descripcion: "actualizar-autor", "PUT"));

                autorDTO.Enlaces.Add(new DatoHATEOAS(Url.Link("eliminarAutorv1", new { id = autorDTO.Id }), descripcion: "eliminar-autor", "DELETE"));

            }
        }
     
    }
}
