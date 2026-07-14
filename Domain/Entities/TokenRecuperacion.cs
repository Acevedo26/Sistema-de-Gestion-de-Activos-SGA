using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    [Table("tokens_recuperacion")]
    public class TokenRecuperacion
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El usuario asociado es obligatorio.")]
        [Column("usuario_id")]
        public Guid UsuarioId { get; set; }

        [Required(ErrorMessage = "El token es obligatorio.")]
        [MaxLength(200, ErrorMessage = "El token no puede exceder los 200 caracteres.")]
        [Column("token")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de expiración es obligatoria.")]
        [Column("fecha_expiracion")]
        public DateTime FechaExpiracion { get; set; }

        [Required(ErrorMessage = "El estado de utilización es obligatorio.")]
        [Column("utilizado")]
        public bool Utilizado { get; set; } = false;

        // Navegación
        [ForeignKey(nameof(UsuarioId))]
        public Usuario Usuario { get; set; } = null!;
    }
}
