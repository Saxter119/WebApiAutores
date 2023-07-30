using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webAPIAutores.Entidades;
using webAPIAutores.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using webAPIAutores.Servicios;
using webAPIAutores.Utilidades;

namespace webAPIAutores.Controllers.v1
{

    [ApiController]
    [Route("api/autores")]
    [CabezeraEsta("cabezzzera", "1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;    
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(ApplicationDbContext dbContext, IMapper mapper, IAuthorizationService authorizationService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }

        /// <summary>
        /// La lista de todos los autores
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet(Name = "obtenerAutoresv1")] //api/autores
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] PaginacionDTO paginacionDTO)
        {

            var queryable = dbContext.Autores.AsQueryable();

            await HttpContext.InsertarParametrosPaginacionEnCabezera(queryable);

            var autores = await queryable.OrderBy(autor=> autor.Nombre).Paginar(paginacionDTO).ToListAsync();
            var dtos = mapper.Map<List<AutorDTO>>(autores);

            return Ok(dtos);
        }
        /// <summary>
        /// Busca autores
        /// </summary>
        /// <param name="nombre">Ingresa las letras del nombre/s que buscas</param>
        /// <returns></returns>
        
        
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        [HttpGet("{nombre}", Name = "obtenerAutorPorNombrev1")]
        public async Task<ActionResult<List<AutorDTO>>> BuscarPorNombre([FromRoute] string nombre)
        {
            var autores = await dbContext.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();

            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpGet("{id:int}", Name = "obtenerAutorv1")]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        [AllowAnonymous]
        public async Task<ActionResult<AutorDTOConLibros>> BuscarPorId(int id)
        {
            var autor = await dbContext.Autores.
            Include(autorDB => autorDB.LibrosDeAutor).ThenInclude(librosAutor => librosAutor.Libro)
            .FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<AutorDTOConLibros>(autor);

            return dto;
        }


        [HttpPost(Name = "crearAutorv1")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdminNoBulto")]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO) //Espera informacion de tipo autor
        {
            var autorExiste = await dbContext.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);//controller validation

            if (autorExiste)
            {
                return BadRequest($"Ya existe un autor con el nombre '{autorCreacionDTO.Nombre}' mi pana");
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);
            dbContext.Add(autor);
            await dbContext.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("obtenerAutorv1", new { id = autor.Id }, autorDTO);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdminNoBulto")]
        [HttpPut("{id:int}", Name = "actualizarAutorv1")]
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
        {
            var existe = await dbContext.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;

            dbContext.Update(autor); //EF no utiliza repositorios?
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdminNoBulto")]
        [HttpDelete("{id:int}", Name = "eliminarAutorv1")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await dbContext.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            dbContext.Remove(new Autor() { Id = id });
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }

}