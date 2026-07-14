using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
    [Table("activos")]
    public class Activo
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        [Column("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [MaxLength(150)]
        [Column("proveedor")]
        public string? Proveedor { get; set; }

        [Required]
        [Column("costo_inicial", TypeName = "decimal(18,2)")]
        public decimal CostoInicial { get; set; }

        [Required]
        [Column("fecha_adquisicion")]
        public DateTime FechaAdquisicion { get; set; }

        [MaxLength(100)]
        [Column("numero_serie")]
        public string? NumeroSerie { get; set; }

        [Required]
        [Column("estado")]
        public EstadoActivo Estado { get; set; } = EstadoActivo.Activo;

        [Required]
        [Column("categoria_id")]
        public Guid CategoriaId { get; set; }

        [Required]
        [Column("ubicacion_id")]
        public Guid UbicacionId { get; set; }

        // Navegación
        [ForeignKey(nameof(CategoriaId))]
        public Categoria Categoria { get; set; } = null!;

        [ForeignKey(nameof(UbicacionId))]
        public Ubicacion Ubicacion { get; set; } = null!;

        public Depreciacion? Depreciacion { get; set; }
    }
}
