using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Auth
{
    public class RestablecerPasswordDto
    {
        [Required(ErrorMessage = "El token es obligatorio.")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string NuevaContrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmación de la contraseña es obligatoria.")]
        [Compare(nameof(NuevaContrasena), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContrasena { get; set; } = string.Empty;
    }
}
