using System;
using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    public class TokenRecuperacion
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public Guid UsuarioId { get; set; }
        
        [Required]
        [MaxLength(256)]
        public string Token { get; set; } = string.Empty;
        
        [Required]
        public DateTime FechaExpiracion { get; set; }
        
        public bool Usado { get; set; } = false;

        public virtual Usuario Usuario { get; set; } = null!;
    }
}
