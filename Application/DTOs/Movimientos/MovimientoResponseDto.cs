namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Movimientos
{
    public class MovimientoResponseDto
    {
        public Guid Id { get; set; }

        public Guid ActivoId { get; set; }

        public Guid UbicacionOrigenId { get; set; }

        public Guid UbicacionDestinoId { get; set; }

        public DateTime FechaMovimiento { get; set; }

        public string? Observaciones { get; set; }
    }
}