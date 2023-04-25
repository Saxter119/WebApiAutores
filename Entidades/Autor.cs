using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using webAPIAutores.DTOs;
using webAPIAutores.Validaciones;

namespace webAPIAutores.Entidades
{
    public class Autor 
    {
        public int Id { get; set; }
        [CapLetter]
        [Required(ErrorMessage = "Ta vacío el campo '{0}'")]
        [StringLength(maximumLength: 120, MinimumLength = 3, ErrorMessage = "El campo '{0}' debe estár entre {2} y {1} carácteres")]
        public string Nombre { get; set; }
        public List<AutorLibro> LibrosDeAutor { get; set; }
        }
    }
