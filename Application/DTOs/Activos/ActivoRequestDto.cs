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
        public decimal CostoInicial { get; set; }

        public string? Proveedor { get; set; }

        public string? NumeroSerie { get; set; }
    }
}

