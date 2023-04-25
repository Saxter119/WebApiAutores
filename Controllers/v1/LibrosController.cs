using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webAPIAutores.DTOs;
using webAPIAutores.Entidades;

namespace webAPIAutores.Controllers.v1
{
    [ApiController]
    [Route("api/v1/libros")] //poner 'api' es opcional
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("all", Name ="obtenerLibros")]
        public async Task<List<BookDTO>> GetAll()
        {

            var libros = await dbContext.Libro.ToListAsync(); //include the autor names


            var bookDto = mapper.Map<List<BookDTO>>(libros);

            return bookDto;
        }

/// <summary>
/// Incluye los dos primeros comentarios de la lista
/// </summary>
/// <param name="id">Introduce el Id del libro</param>
/// <returns></returns>
        [HttpGet("{id:int}", Name = "obtenerLibro")]
        public async Task<ActionResult<BookDTOWithAutor>> Get(int id)
        {
            var book = await dbContext.Libro.
            Include(libroDB => libroDB.AutoresDeLibro).ThenInclude(AutoresLibros => AutoresLibros.Autor).Include(x => x.Comentarios.Take(2)).FirstOrDefaultAsync(x => x.Id == id); 

            if (book == null)
            {
                return BadRequest("No existe un libro con ese id");
            }

            book.AutoresDeLibro = book.AutoresDeLibro.OrderBy(X => X.Orden).ToList();

            return mapper.Map<BookDTOWithAutor>(book);

        }

        [HttpPost(Name ="crearLibro")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdminNoBulto")]
        public async Task<ActionResult> Post(BookCreationDTO bookCreationDTO)
        {

            if ((bookCreationDTO.AutoresIds == null))
            {
                return BadRequest("No se puede crear un libro sin autores");
            }


            var autoresIds = await dbContext.Autores.Where(autoresDB => bookCreationDTO.AutoresIds.Contains(autoresDB.Id)).Select(x => x.Id).ToListAsync();

            if (bookCreationDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("Parece que algúno de los autores no existe");
            }

            var book = mapper.Map<Libro>(bookCreationDTO);
            AsingnAutorsOrder(book);

            dbContext.Add(book);
            await dbContext.SaveChangesAsync();

            var libroDTO = mapper.Map<BookDTO>(book);

            return CreatedAtRoute("ObtenerLibro", new { id = book.Id }, libroDTO);
        }

        [HttpPut("{id:int}", Name ="actualizarLibro")]
        public async Task<ActionResult> Put(BookCreationDTO bookCreationDTO, int id)
        {
            var libroDB = await dbContext.Libro.Include(libroDB => libroDB.AutoresDeLibro).FirstOrDefaultAsync(libroDB => libroDB.Id == id);

            if (libroDB == null)
            {
                return NotFound("¿Y ese Id to' raro pana mío?");
            }

            libroDB = mapper.Map(bookCreationDTO, libroDB);
            AsingnAutorsOrder(libroDB);
            await dbContext.SaveChangesAsync();
            return NoContent();

        }
        [HttpPatch(Name ="modificarLibro")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<BookPatchDto> jsonPatchDocument)
        {
            if (jsonPatchDocument == null)
            {
                return BadRequest();
            }

            var libroDB = await dbContext.Libro.FirstOrDefaultAsync(x => x.Id == id);

            if (libroDB == null)
            {
                return NotFound();
            }

            var libroDTO = mapper.Map<BookPatchDto>(libroDB);

            jsonPatchDocument.ApplyTo(libroDTO, ModelState);

            var esValido = TryValidateModel(libroDTO);

            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(libroDTO, libroDB);

            await dbContext.SaveChangesAsync();

            return NoContent();


        }


        private void AsingnAutorsOrder(Libro book)
        {
            if (book.AutoresDeLibro != null)
            {
                for (int i = 0; i < book.AutoresDeLibro.Count; i++)
                {
                    book.AutoresDeLibro[i].Orden = i;
                }
            }
        }
        [HttpDelete("{id:int}", Name ="eliminarLibro")]
        public async Task<ActionResult> Delete(int id)
        {
            var existBook = await dbContext.Libro.AnyAsync(x => x.Id == id);

            if (!existBook)
            {
                return BadRequest("No existe un libro con ese id");
            }

            dbContext.Remove(new Libro() { Id = id });
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}