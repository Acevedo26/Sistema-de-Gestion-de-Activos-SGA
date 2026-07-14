using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Activos
{
    public class ActivoUpdateDto
    {
        public string? Descripcion { get; set; }

        public Guid? CategoriaId { get; set; }

        public Guid? UbicacionId { get; set; }

        public decimal? CostoInicial { get; set; }

        public string? Proveedor { get; set; }

        public string? NumeroSerie { get; set; }

        public EstadoActivo? Estado { get; set; }
    }
}