using System.ComponentModel.DataAnnotations.Schema;
using libreriawebpractica.Models;
using Microsoft.AspNetCore.Http;

namespace libreriawebpractica.Models
{
    public class Libro
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int AutorId { get; set; }
        public Autor? Autor { get; set; } // <-- HAZLA OPCIONAL
        public string? ImagenRuta { get; set; } // <-- HAZLA OPCIONAL

        [NotMapped]
        public IFormFile? ImagenArchivo { get; set; }
    }
}