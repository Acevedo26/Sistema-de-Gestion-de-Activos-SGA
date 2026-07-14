using System;
using System.ComponentModel.DataAnnotations;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Usuarios
{
    public class UsuarioRequestDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [MaxLength(150)]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [MaxLength(300)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El RolId es obligatorio.")]
        public Guid RolId { get; set; }

        public EstadoUsuario Estado { get; set; } = EstadoUsuario.Activo;
    }
}
