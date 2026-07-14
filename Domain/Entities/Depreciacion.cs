using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    [Table("depreciaciones")]
    public class Depreciacion
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("activo_id")]
        public Guid ActivoId { get; set; }

        [Required]
        [Column("vida_util_asignada")]
        public int VidaUtilAsignada { get; set; }

        [Required]
        [Column("valor_residual", TypeName = "decimal(18,2)")]
        public decimal ValorResidual { get; set; } = 0m;

        [Required]
        [Column("valor_actual", TypeName = "decimal(18,2)")]
        public decimal ValorActual { get; set; }

        [Required]
        [Column("porcentaje_consumido", TypeName = "decimal(5,2)")]
        public decimal PorcentajeConsumido { get; set; } = 0m;

        [Required]
        [Column("fecha_ultimo_calculo")]
        public DateTime FechaUltimoCalculo { get; set; }

        // Navegación
        [ForeignKey(nameof(ActivoId))]
        public Activo Activo { get; set; } = null!;

        public ICollection<HistorialDepreciacion> Historial { get; set; } = new List<HistorialDepreciacion>();
    }
}
