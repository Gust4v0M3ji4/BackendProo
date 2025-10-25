using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherBroadcast.Data;
using WeatherBroadcast.Models;
using WeatherBroadcast.Services;

namespace WeatherBroadcast.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly WeatherDbContext _context;
        private readonly JwtServices _jwtService;

        public AuthController(WeatherDbContext context, JwtServices jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Usuario credenciales)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == credenciales.Email && u.Password == credenciales.Password);
            if (usuario == null)
            {
                return Unauthorized("Credenciales Invalidas");
            }

            var token = _jwtService.GenerarToken(usuario);
            return Ok(new { token });
        }
        
        
        
    }
}
