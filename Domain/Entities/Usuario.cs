using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

// <summary>
// Entidad Usuario
// </summary>
namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(150, ErrorMessage = "El nombre no puede exceder los 150 caracteres.")]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [MaxLength(150, ErrorMessage = "El correo no puede exceder los 150 caracteres.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico es inválido.")]
        [Column("correo")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(300, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.", MinimumLength = 8)]
        [PasswordPropertyText]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [Column("rol_id")]
        public Guid RolId { get; set; }

        [Required(ErrorMessage = "El estado del usuario es obligatorio.")]
        [Column("estado")]
        public EstadoUsuario Estado { get; set; } = EstadoUsuario.Activo;

        // Navegación
        [ForeignKey(nameof(RolId))]
        public Rol Rol { get; set; } = null!;
    }
}
