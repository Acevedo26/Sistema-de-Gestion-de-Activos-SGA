using System;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Usuarios
{
    public class UsuarioResponseDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public Guid RolId { get; set; }
        public EstadoUsuario Estado { get; set; }
    }
}
