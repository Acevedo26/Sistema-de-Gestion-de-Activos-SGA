using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    [Table("notificaciones")]
    public class Notificacion
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("tipo")]
        public TipoNotificacion Tipo { get; set; }

        [Column("activo_id")]
        public Guid? ActivoId { get; set; }

        [Column("programa_mantenimiento_id")]
        public Guid? ProgramaMantenimientoId { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("mensaje")]
        public string Mensaje { get; set; } = string.Empty;

        [Required]
        [Column("fecha_generacion")]
        public DateTime FechaGeneracion { get; set; }

        [Required]
        [Column("leida")]
        public bool Leida { get; set; } = false;

        [Required]
        [Column("enviada_por_correo")]
        public bool EnviadaPorCorreo { get; set; } = false;

        // Navegación
        [ForeignKey(nameof(ActivoId))]
        public Activo? Activo { get; set; }

        [ForeignKey(nameof(ProgramaMantenimientoId))]
        public ProgramaMantenimiento? ProgramaMantenimiento { get; set; }
    }
}
