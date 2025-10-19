using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace libreriawebpractica.Models
{
    public class Autor
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public ICollection<Libro> Libros { get; set; } = new List<Libro>();
    }
}