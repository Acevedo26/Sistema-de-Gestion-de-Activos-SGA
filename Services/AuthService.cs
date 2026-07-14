using System;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Domain.Entities;
using Sistema_de_Gestion_de_Activos.Repositories.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRepository<TokenRecuperacion> _tokenRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUsuarioRepository usuarioRepository,
            IRepository<TokenRecuperacion> tokenRepository,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _tokenRepository = tokenRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task SolicitarRecuperacionAsync(string correo)
        {
            var usuario = await _usuarioRepository.ObtenerPorCorreoAsync(correo);
            
            // RF-19: No revelar si el correo existe, si no existe retornamos silenciosamente.
            if (usuario == null)
            {
                return;
            }

            // Invalida tokens previos no usados
            var tokensActivos = await _tokenRepository.FindAsync(t => t.UsuarioId == usuario.Id && !t.Utilizado && t.FechaExpiracion > DateTime.UtcNow);
            foreach (var t in tokensActivos)
            {
                t.Utilizado = true;
                _tokenRepository.Update(t);
            }

            // Generar nuevo token seguro
            string rawToken = Guid.NewGuid().ToString("N"); // 32 caracteres sin guiones
            
            var tokenEntity = new TokenRecuperacion
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuario.Id,
                Token = rawToken,
                FechaExpiracion = DateTime.UtcNow.AddMinutes(30), // Expira en 30 minutos
                Utilizado = false
            };

            await _tokenRepository.AddAsync(tokenEntity);
            await _tokenRepository.SaveChangesAsync();

            string appUrl = _configuration["AppUrl"] ?? "http://localhost:5000";
            string enlace = $"{appUrl}/auth/restablecer?token={rawToken}";
            
            await _emailService.EnviarCorreoRecuperacionAsync(correo, enlace);
        }

        public async Task<bool> RestablecerContrasenaAsync(string token, string nuevaContrasena)
        {
            var tokenEntities = await _tokenRepository.FindAsync(t => t.Token == token && !t.Utilizado);
            var tokenEntity = tokenEntities.FirstOrDefault();

            if (tokenEntity == null || tokenEntity.FechaExpiracion < DateTime.UtcNow)
            {
                return false; // Token inválido o expirado
            }

            var usuario = await _usuarioRepository.GetByIdAsync(tokenEntity.UsuarioId);
            if (usuario == null)
            {
                return false;
            }

            // Hashear nueva contraseña
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(nuevaContrasena);
            usuario.PasswordHash = hashedPassword;
            _usuarioRepository.Update(usuario);

            // Marcar token como utilizado
            tokenEntity.Utilizado = true;
            _tokenRepository.Update(tokenEntity);

            await _usuarioRepository.SaveChangesAsync(); 

            return true;
        }

        public async Task<Usuario?> ValidarCredencialesAsync(string correo, string password)
        {
            var usuario = await _usuarioRepository.ObtenerPorCorreoAsync(correo);
            if (usuario == null || usuario.Estado != Sistema_de_Gestion_de_Activos.Domain.Enums.EstadoUsuario.Activo)
            {
                return null;
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);
            return isValid ? usuario : null;
        }
    }
}
