using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Activos
{
    public class ActivoResponseDto
    {
        public Guid Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public Guid CategoriaId { get; set; }

        public Guid UbicacionId { get; set; }

        public DateTime FechaAdquisicion { get; set; }

        public decimal CostoInicial { get; set; }

        public string? Proveedor { get; set; }

        public string? NumeroSerie { get; set; }

        public EstadoActivo Estado { get; set; }
    }
}
