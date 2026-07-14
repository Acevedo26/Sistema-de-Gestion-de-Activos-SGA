using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Usuarios;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioResponseDto> CrearUsuarioAsync(UsuarioRequestDto dto);
        Task<UsuarioResponseDto?> ActualizarUsuarioAsync(Guid id, UsuarioUpdateDto dto);
        Task<bool> DesactivarUsuarioAsync(Guid id);
        Task<bool> ActivarUsuarioAsync(Guid id);
        Task<IEnumerable<UsuarioResponseDto>> ObtenerTodosAsync();
        Task<IEnumerable<RolResponseDto>> ObtenerRolesAsync();
    }
}
