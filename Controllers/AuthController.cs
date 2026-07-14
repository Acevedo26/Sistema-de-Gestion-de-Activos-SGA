using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Auth;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using System.Threading.Tasks;

namespace Sistema_de_Gestion_de_Activos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous] // RF-19: Disponible sin autenticación
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("olvide-password")]
        public async Task<IActionResult> OlvidePassword([FromBody] SolicitarRecuperacionDto dto)
        {
            // Siempre responde 200 OK genérico para no revelar si el correo existe
            await _authService.SolicitarRecuperacionAsync(dto.Correo);
            
            return Ok(new { mensaje = "Si el correo existe en nuestro sistema, recibirá un enlace para restablecer su contraseña en breve." });
        }

        [HttpPost("restablecer-password")]
        public async Task<IActionResult> RestablecerPassword([FromBody] RestablecerPasswordDto dto)
        {
            var exito = await _authService.RestablecerContrasenaAsync(dto.Token, dto.NuevaContrasena);

            if (!exito)
            {
                return BadRequest(new { mensaje = "El token es inválido o ha expirado." });
            }

            return Ok(new { mensaje = "Su contraseña ha sido restablecida exitosamente." });
        }
    }
}
