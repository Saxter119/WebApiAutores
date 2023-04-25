using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webAPIAutores.DTOs;

namespace webAPIAutores.Controllers.v1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RutaController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RutaController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "obtenerRutas")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> GetRoutes()
        {
            var hateoas = new List<DatoHATEOAS>();

            var esAdmin = await authorizationService.AuthorizeAsync(User, "EsAdminNoBulto");

            //hateoas.Add(new DatoHATEOAS(Url.Link("obtenerAutor", new { }), descripcion: "autor", "GET"));

            hateoas.Add(new DatoHATEOAS(Url.Link("obtenerRutas", new { }), "self", "GET"));

            hateoas.Add(new DatoHATEOAS(Url.Link("obtenerAutores", new { }), "autores", "GET"));

            hateoas.Add(new DatoHATEOAS(Url.Link("obtenerLibros", new { }), "libros", "GET"));

            if (esAdmin.Succeeded)
            {
                hateoas.Add(new DatoHATEOAS(Url.Link("crearAutor", new { }), descripcion: "crear-autor", "POST"));

                hateoas.Add(new DatoHATEOAS(Url.Link("crearLibro", new { }), descripcion: "crear-libro", "POST"));
            }


            return hateoas;
        }
    }
}