
using AutoMapper;
using webAPIAutores.DTOs;
using webAPIAutores.Entidades;

namespace webAPIAutores.Utilidades
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AutorCreacionDTO, Autor>();
            CreateMap<Autor, AutorDTO>();
            CreateMap<Autor, AutorDTOConLibros>()
            .ForMember(autorDTO => autorDTO.Libros, opciones => opciones.MapFrom(MapAutorBooksDTO));

            CreateMap<BookCreationDTO, Libro>()
            .ForMember(libro => libro.AutoresDeLibro, opciones => opciones.MapFrom(MapAutoresLibros));
            CreateMap<Libro, BookDTO >();
            CreateMap<Libro, BookDTOWithAutor>()
            .ForMember(bookDTO => bookDTO.Autores, opciones => opciones.MapFrom(MapLibroAutoresDTO));
            CreateMap<BookPatchDto, Libro>().ReverseMap();


            CreateMap<ComentarioCreacionDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();
            //CreateMap<>

        }

        private List<BookDTO> MapAutorBooksDTO(Autor autor, AutorDTO autorDTO)
        {
            var resultado = new List<BookDTO>();

            if (autor.LibrosDeAutor == null) { return resultado; }

            foreach (var libroDeAutor in autor.LibrosDeAutor)
            {
                resultado.Add(new BookDTO()
                {
                    Id = libroDeAutor.LibroId,
                    Titulo = libroDeAutor.Libro.Titulo
                });
            }

            return resultado;
        }

        private List<AutorLibro> MapAutoresLibros(BookCreationDTO bookCreationDTO, Libro libro)
        {
            var resultado = new List<AutorLibro>();

            if (bookCreationDTO.AutoresIds == null) { return resultado; }


            foreach (var autorid in bookCreationDTO.AutoresIds)
            {
                resultado.Add(new AutorLibro() { AutorId = autorid });
            }

            return resultado;
        }

        private List<AutorDTO> MapLibroAutoresDTO(Libro libro, BookDTO bookDTO)
        {
            var resultado = new List<AutorDTO>();

            if (libro.AutoresDeLibro == null) { return resultado; }

            foreach (var autor in libro.AutoresDeLibro)
            {
                resultado.Add(new AutorDTO()
                {
                    Id = autor.AutorId,
                    Nombre = autor.Autor.Nombre
                });
            }

            return resultado;
        }
    }
}