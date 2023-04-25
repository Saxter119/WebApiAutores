using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webAPIAutores.DTOs;
using webAPIAutores.Entidades;
using webAPIAutores.Utilidades;
//using Microsoft.AspNetCore.Components; Esto no se estaba utilizando
//using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute; , cuando algo es ambiguo, esta puede ser una solucion. Especificando que anotacion usará en caso de que no se quiera eliminar ninguna librería.

namespace webAPIAutores.Controllers.v1
{
    [ApiController]
    [Route("api/v1/libros/{libroId:int}/comentario")]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(ApplicationDbContext applicationDbContext, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.applicationDbContext = applicationDbContext;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet(Name ="obtenerComentarioDeLibro")]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId, [FromQuery] PaginacionDTO paginacionDTO)
        {
            var existeLibro = await applicationDbContext.Libro.AnyAsync(libro => libro.Id == libroId);

            if (!existeLibro)
            {
                return NotFound("No se ha encontrado ningun libro con ese ID");
            }
            //-----------------------------------------------------------------------------------------------------------------------------
            var queryable = applicationDbContext.Comentarios.Where(libro => libro.LibroId == libroId).OrderBy(id=> id.Id).AsQueryable();

            await HttpContext.InsertarParametrosPaginacionEnCabezera(queryable);

            var comentarios = await queryable.Paginar(paginacionDTO).ToListAsync();

            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id:int}", Name ="ObtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> GetById(int id)
        { 
            var comentario = await applicationDbContext.Comentarios.FirstOrDefaultAsync(comentarioDB=> comentarioDB.Id == id);

            if(comentario == null){return NotFound();}

            return mapper.Map<ComentarioDTO>(comentario);
            
        }

        [HttpPost(Name ="crearComentario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where(claim=> claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;

            var existeLibro = await applicationDbContext.Libro.AnyAsync(libro => libro.Id == libroId);

            if (!existeLibro)
            {
                return NotFound("No se ha encontrado ningun libro con ese ID");
            }

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = libroId;
            comentario.UsuarioId = usuarioId;

            applicationDbContext.Add(comentario);
            await applicationDbContext.SaveChangesAsync();
    
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("ObtenerComentario", new {libroId = libroId, id = comentario.Id}, comentarioDTO);
        }
        [HttpPut("{id:int}", Name ="actualizarComentario")]
        public async Task<ActionResult> Put(ComentarioCreacionDTO comentarioCreacionDTO, int libroId, int id)
        {
            var existBook = await applicationDbContext.Libro.AnyAsync(libroBD=> libroBD.Id == id);

            if(!existBook)
            {
                return NotFound("No existe un libro con ese Id");
            }

            var existComment = await applicationDbContext.Comentarios.AnyAsync(comentarioDB=> comentarioDB.Id== id);

            if(!existComment)
            {
             return NotFound("No existe un comentario con ese Id");   
            }
            
            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.Id = id;
            comentario.LibroId = libroId;
            applicationDbContext.Update(comentario);
            await applicationDbContext.SaveChangesAsync();
            return NoContent();

        }
    }
}