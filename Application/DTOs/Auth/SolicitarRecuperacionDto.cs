using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Auth
{
    public class SolicitarRecuperacionDto
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        public string Correo { get; set; } = string.Empty;
    }
}

public class LoginDto
{
    [Required, EmailAddress]
    public string Correo { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiraEnUtc { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}
