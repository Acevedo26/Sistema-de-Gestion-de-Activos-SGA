using System.Collections.Generic;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Activos
{
    public class ActivoDetalleCompletoDto : ActivoResponseDto
    {
        public List<MantenimientoResumenDto> Mantenimientos { get; set; } = new List<MantenimientoResumenDto>();
        public List<MovimientoResumenDto> Movimientos { get; set; } = new List<MovimientoResumenDto>();
        public AnalisisRendimientoDto Analisis { get; set; } = new AnalisisRendimientoDto();
    }
}
