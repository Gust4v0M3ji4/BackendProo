// File: WeatherForecast.cs
using System.ComponentModel.DataAnnotations; // Importa para usar Data Annotations
using System.ComponentModel.DataAnnotations.Schema; // Opcional, para configuraciones avanzadas

namespace WeatherBroadcast // Asegúrate del namespace correcto
{
    public class WeatherForecast
    {
        [Key] // Define Id como clave primaria
        public int Id { get; set; }

        [Required] // Hace que el campo sea obligatorio
        [MaxLength(255)] // Limita la longitud del campo
        public string Location { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        [MaxLength(500)] // Limita la longitud del campo
        public string? Summary { get; set; }
        // Puedes agregar más propiedades según necesites (humedad, viento, etc.)
    }
}