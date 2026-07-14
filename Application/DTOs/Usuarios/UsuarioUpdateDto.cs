using System;
using System.ComponentModel.DataAnnotations;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Usuarios
{
    public class UsuarioUpdateDto
    {
        [MaxLength(150)]
        public string? Nombre { get; set; }

        public Guid? RolId { get; set; }

        public EstadoUsuario? Estado { get; set; }
    }
}
