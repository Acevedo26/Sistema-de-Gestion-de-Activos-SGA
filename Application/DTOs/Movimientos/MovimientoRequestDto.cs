using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Movimientos
{
    public class MovimientoRequestDto
    {
        [Required]
        public Guid ActivoId { get; set; }

        [Required]
        public Guid UbicacionOrigenId { get; set; }

        [Required]
        public Guid UbicacionDestinoId { get; set; }

        public string? Observaciones { get; set; }
    }
}