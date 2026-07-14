using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    [Table("movimientos")]
    public class Movimiento
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column("activo_id")]
        public Guid ActivoId { get; set; }

        [Required]
        [Column("ubicacion_origen_id")]
        public Guid UbicacionOrigenId { get; set; }

        [Required]
        [Column("ubicacion_destino_id")]
        public Guid UbicacionDestinoId { get; set; }

        [Required]
        [Column("fecha_movimiento")]
        public DateTime FechaMovimiento { get; set; }

        [MaxLength(250)]
        [Column("observaciones")]
        public string? Observaciones { get; set; }

        [ForeignKey(nameof(ActivoId))]
        public Activo Activo { get; set; } = null!;

        [ForeignKey(nameof(UbicacionOrigenId))]
        public Ubicacion UbicacionOrigen { get; set; } = null!;

        [ForeignKey(nameof(UbicacionDestinoId))]
        public Ubicacion UbicacionDestino { get; set; } = null!;
    }
}