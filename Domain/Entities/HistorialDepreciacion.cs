using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    [Table("historial_depreciacion")]
    public class HistorialDepreciacion
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "La depreciación asociada es obligatoria.")]
        [Column("depreciacion_id")]
        public Guid DepreciacionId { get; set; }

        [Required(ErrorMessage = "La fecha de consulta es obligatoria.")]
        [Column("fecha_consulta")]
        public DateTime FechaConsulta { get; set; }

        [Required(ErrorMessage = "El valor actual es obligatorio.")]
        [Column("valor_actual", TypeName = "decimal(18,2)")]
        public decimal ValorActual { get; set; }

        [Required(ErrorMessage = "El valor residual es obligatorio.")]
        [Column("valor_residual", TypeName = "decimal(18,2)")]
        public decimal ValorResidual { get; set; }

        [Required(ErrorMessage = "El porcentaje consumido es obligatorio.")]
        [Column("porcentaje_consumido", TypeName = "decimal(5,2)")]
        public decimal PorcentajeConsumido { get; set; }

        [Required(ErrorMessage = "La depreciación acumulada es obligatoria.")]
        [Column("depreciacion_acumulada", TypeName = "decimal(18,2)")]
        public decimal DepreciacionAcumulada { get; set; }

        // Navegación
        [ForeignKey(nameof(DepreciacionId))]
        public Depreciacion Depreciacion { get; set; } = null!;
    }
}
