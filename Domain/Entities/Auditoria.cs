using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    [Table("auditoria")]
    public class Auditoria
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("usuario_id")]
        public Guid? UsuarioId { get; set; }

        [Required(ErrorMessage = "La tabla afectada es obligatoria.")]
        [Column("tabla_afectada")]
        public string TablaAfectada { get; set; } = string.Empty;

        [Required(ErrorMessage = "La acción es obligatoria.")]
        [Column("accion")]
        public AccionAuditoria Accion { get; set; }

        [Column("valores_anteriores")]
        public string? ValoresAnteriores { get; set; }

        [Column("valores_nuevos")]
        public string? ValoresNuevos { get; set; }

        [Required(ErrorMessage = "La fecha de la acción es obligatoria.")]
        [Column("fecha_accion")]
        public DateTime FechaAccion { get; set; }

        // Navegación
        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }
    }
}
