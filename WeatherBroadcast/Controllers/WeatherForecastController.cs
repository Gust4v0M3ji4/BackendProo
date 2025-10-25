using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherBroadcast.Data;
using WeatherBroadcast;
using Microsoft.AspNetCore.Authorization;

namespace WeatherBroadcast.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherDbContext _context;

        public WeatherController(WeatherDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> Get()
        {
            var forecasts = await _context.WeatherForecasts.ToListAsync();
            return forecasts;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WeatherForecast>> GetById(int id)
        {
            var forecast = await _context.WeatherForecasts.FindAsync(id);

            if (forecast == null)
            {
                return NotFound($"No se encontró el pronóstico con ID {id}.");
            }

            return forecast;
        }

        [HttpPost]
        public async Task<ActionResult<WeatherForecast>> Post([FromBody] WeatherForecast newForecast)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.WeatherForecasts.Add(newForecast);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newForecast.Id }, newForecast);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] WeatherForecast updatedForecast)
        {
            if (id != updatedForecast.Id)
            {
                return BadRequest("El ID en la URL no coincide con el ID del objeto enviado.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(updatedForecast).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.WeatherForecasts.Any(e => e.Id == id))
                {
                    return NotFound($"No se encontró el pronóstico con ID {id} para actualizar.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var forecast = await _context.WeatherForecasts.FindAsync(id);
            if (forecast == null)
            {
                return NotFound($"No se encontró el pronóstico con ID {id} para eliminar.");
            }

            _context.WeatherForecasts.Remove(forecast);
            await _context.SaveChangesAsync();

            return NoContent();
        } 


    }
}