using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Activos
{
    public class ActivoRequestDto
    {
        [Required]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public Guid CategoriaId { get; set; }

        [Required]
        public Guid UbicacionId { get; set; }

        [Required]
        public DateTime FechaAdquisicion { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El costo inicial debe ser mayor que cero.")]
        public decimal CostoInicial { get; set; }

        public string? Proveedor { get; set; }

        public string? NumeroSerie { get; set; }

        public Guid? ResponsableId { get; set; }
        [MaxLength(500)]
        public string? ObservacionesAsignacion { get; set; }
    }
}

