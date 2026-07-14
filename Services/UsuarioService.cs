using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Usuarios;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Domain.Entities;
using Sistema_de_Gestion_de_Activos.Domain.Enums;
using Sistema_de_Gestion_de_Activos.Repositories.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly Sistema_de_Gestion_de_Activos.Data.SgaDbContext _dbContext;

        public UsuarioService(IUsuarioRepository usuarioRepository, Sistema_de_Gestion_de_Activos.Data.SgaDbContext dbContext)
        {
            _usuarioRepository = usuarioRepository;
            _dbContext = dbContext;
        }

        public async Task<UsuarioResponseDto> CrearUsuarioAsync(UsuarioRequestDto dto)
        {
            // Validar unicidad de correo
            if (await _usuarioRepository.ExisteCorreoAsync(dto.Correo))
            {
                throw new InvalidOperationException("El correo ya está en uso.");
            }

            // Hashear contraseña usando BCrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                PasswordHash = hashedPassword,
                RolId = dto.RolId,
                Estado = dto.Estado
            };

            await _usuarioRepository.AddAsync(usuario);
            await _usuarioRepository.SaveChangesAsync();

            return new UsuarioResponseDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                RolId = usuario.RolId,
                Estado = usuario.Estado
            };
        }

        public async Task<UsuarioResponseDto?> ActualizarUsuarioAsync(Guid id, UsuarioUpdateDto dto)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(dto.Nombre)) usuario.Nombre = dto.Nombre;
            if (dto.RolId.HasValue) usuario.RolId = dto.RolId.Value;
            if (dto.Estado.HasValue) usuario.Estado = dto.Estado.Value;

            _usuarioRepository.Update(usuario);
            await _usuarioRepository.SaveChangesAsync();

            return new UsuarioResponseDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                RolId = usuario.RolId,
                Estado = usuario.Estado
            };
        }

        public async Task<bool> DesactivarUsuarioAsync(Guid id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) return false;

            usuario.Estado = EstadoUsuario.Inactivo; // Desactivación lógica
            _usuarioRepository.Update(usuario);
            await _usuarioRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ActivarUsuarioAsync(Guid id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null) return false;

            usuario.Estado = EstadoUsuario.Activo;
            _usuarioRepository.Update(usuario);
            await _usuarioRepository.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<UsuarioResponseDto>> ObtenerTodosAsync()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return usuarios.Select(u => new UsuarioResponseDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Correo = u.Correo,
                RolId = u.RolId,
                Estado = u.Estado
            });
        }

        public async Task<IEnumerable<RolResponseDto>> ObtenerRolesAsync()
        {
            var roles = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(_dbContext.Roles);
            return roles.Select(r => new RolResponseDto
            {
                Id = r.Id,
                Nombre = r.Nombre.ToString()
            });
        }
    }
}
