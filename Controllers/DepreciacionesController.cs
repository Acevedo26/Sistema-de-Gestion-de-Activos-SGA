using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Controllers
{
    [ApiController]
    [Route("api/activos/{activoId}/depreciacion")]
    public class DepreciacionesController : ControllerBase
    {
        private readonly IDepreciacionService _depreciacionService;
        private readonly IHistorialDepreciacionService _historialService;
        private readonly IAnalisisIntegradoService _analisisService;

        public DepreciacionesController(
            IDepreciacionService depreciacionService,
            IHistorialDepreciacionService historialService,
            IAnalisisIntegradoService analisisService)
        {
            _depreciacionService = depreciacionService;
            _historialService = historialService;
            _analisisService = analisisService;
        }

        // RF-22: Visualización de Valor Actual (y proyecciones)
        [HttpGet]
        public async Task<IActionResult> ObtenerDetalle(Guid activoId)
        {
            var detalle = await _depreciacionService.ObtenerDetalleFinancieroAsync(activoId);
            if (detalle == null) return NotFound("Activo o información de depreciación no encontrada.");

            return Ok(detalle);
        }

        // RF-24: Historial de Depreciación (Snapshots)
        [HttpGet("historial")]
        public async Task<IActionResult> ObtenerHistorial(Guid activoId)
        {
            var historial = await _historialService.ObtenerHistorialPorActivoAsync(activoId);
            return Ok(historial);
        }

        // RF-29: Análisis Costo de Mantenimiento vs Valor Actual
        [HttpGet("analisis-integrado")]
        public async Task<IActionResult> ObtenerAnalisisIntegrado(Guid activoId)
        {
            var relacion = await _analisisService.ObtenerRelacionCostoValorAsync(activoId);
            return Ok(new { ActivoId = activoId, PorcentajeCostoVsValor = relacion });
        }
    }
}
