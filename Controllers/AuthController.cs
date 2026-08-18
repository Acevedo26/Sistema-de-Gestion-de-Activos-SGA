using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Auth;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, [FromServices] IConfiguration configuration)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var usuario = await _authService.ValidarCredencialesAsync(dto.Correo, dto.Password);
            if (usuario == null) return Unauthorized(new { mensaje = "Credenciales inválidas." });

            var settings = configuration.GetSection("JwtSettings");
            var minutes = settings.GetValue<int?>("ExpirationInMinutes") ?? 120;
            var expires = DateTime.UtcNow.AddMinutes(minutes);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings["SecretKey"]!));
            var token = new JwtSecurityToken(
                issuer: settings["Issuer"], audience: settings["Audience"], claims: claims,
                expires: expires, signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return Ok(new LoginResponseDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiraEnUtc = expires,
                Nombre = usuario.Nombre,
                Rol = usuario.Rol.Nombre.ToString()
            });
        }

        [HttpPost("olvide-password")]
        public async Task<IActionResult> OlvidePassword([FromBody] SolicitarRecuperacionDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            // Siempre responde 200 OK genérico para no revelar si el correo existe
            await _authService.SolicitarRecuperacionAsync(dto.Correo);
            
            return Ok(new { mensaje = "Si el correo existe en nuestro sistema, recibirá un enlace para restablecer su contraseña en breve." });
        }

        [HttpPost("restablecer-password")]
        public async Task<IActionResult> RestablecerPassword([FromBody] RestablecerPasswordDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var exito = await _authService.RestablecerContrasenaAsync(dto.Token, dto.NuevaContrasena);

            if (!exito)
            {
                return BadRequest(new { mensaje = "El token es inválido o ha expirado." });
            }

            return Ok(new { mensaje = "Su contraseña ha sido restablecida exitosamente." });
        }
    }
}
