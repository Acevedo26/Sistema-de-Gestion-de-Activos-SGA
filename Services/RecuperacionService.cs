using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Domain.Entities;
using Sistema_de_Gestion_de_Activos.Repositories.Interfaces;
using BCrypt.Net;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class RecuperacionService : IRecuperacionService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRepository<TokenRecuperacion> _tokenRepository;
        private readonly ILogger<RecuperacionService> _logger;

        public RecuperacionService(
            IUsuarioRepository usuarioRepository,
            IRepository<TokenRecuperacion> tokenRepository,
            ILogger<RecuperacionService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _tokenRepository = tokenRepository;
            _logger = logger;
        }

        public async Task SolicitarRecuperacionAsync(string email)
        {
            var usuario = await _usuarioRepository.ObtenerPorCorreoAsync(email);
            if (usuario == null) return;

            var tokensActivos = await _tokenRepository.FindAsync(t => t.UsuarioId == usuario.Id && !t.Usado && t.FechaExpiracion > DateTime.UtcNow);
            foreach (var t in tokensActivos)
            {
                t.Usado = true;
                _tokenRepository.Update(t);
            }

            string rawToken = Guid.NewGuid().ToString("N");
            var tokenEntity = new TokenRecuperacion
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuario.Id,
                Token = rawToken,
                FechaExpiracion = DateTime.UtcNow.AddMinutes(30),
                Usado = false
            };

            await _tokenRepository.AddAsync(tokenEntity);
            await _tokenRepository.SaveChangesAsync();

            _logger.LogInformation($"[RECUPERACION] Token generado para {email}: {rawToken}");
        }

        public async Task<bool> ValidarTokenAsync(string token)
        {
            var tokenEntities = await _tokenRepository.FindAsync(t => t.Token == token && !t.Usado);
            var tokenEntity = tokenEntities.FirstOrDefault();
            return tokenEntity != null && tokenEntity.FechaExpiracion > DateTime.UtcNow;
        }

        public async Task RestablecerPasswordAsync(string token, string nuevaPassword)
        {
            var tokenEntities = await _tokenRepository.FindAsync(t => t.Token == token && !t.Usado);
            var tokenEntity = tokenEntities.FirstOrDefault();

            if (tokenEntity == null || tokenEntity.FechaExpiracion < DateTime.UtcNow)
            {
                throw new Exception("Token inválido o expirado.");
            }

            var usuario = await _usuarioRepository.GetByIdAsync(tokenEntity.UsuarioId);
            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado.");
            }

            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);
            _usuarioRepository.Update(usuario);

            tokenEntity.Usado = true;
            _tokenRepository.Update(tokenEntity);

            await _usuarioRepository.SaveChangesAsync();
            await _tokenRepository.SaveChangesAsync();
        }
    }
}
