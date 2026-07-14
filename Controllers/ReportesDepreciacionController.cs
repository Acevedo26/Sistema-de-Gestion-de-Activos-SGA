using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Controllers
{
    [ApiController]
    [Route("api/reportes/depreciacion")]
    public class ReportesDepreciacionController : ControllerBase
    {
        private readonly IReporteDepreciacionService _reporteService;

        public ReportesDepreciacionController(IReporteDepreciacionService reporteService)
        {
            _reporteService = reporteService;
        }

        // RF-26: Reporte de Depreciación
        [HttpGet]
        public async Task<IActionResult> ReporteGeneral([FromQuery] Guid? categoriaId, [FromQuery] string? estadoDepreciacion)
        {
            var reporte = await _reporteService.GenerarReporteGeneralAsync(categoriaId, estadoDepreciacion);
            return Ok(reporte);
        }

        // RF-27: Análisis de Valor por Departamento
        [HttpGet("valor-por-departamento")]
        public async Task<IActionResult> AnalisisPorDepartamento([FromQuery] Guid? ubicacionId)
        {
            var analisis = await _reporteService.GenerarAnalisisPorDepartamentoAsync(ubicacionId);
            return Ok(analisis);
        }

        // RF-30: Exportación de Reportes Financieros
        [HttpGet("exportar-csv")]
        public async Task<IActionResult> ExportarCsv([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
        {
            if (desde == default || hasta == default || desde > hasta)
                return BadRequest("Rango de fechas inválido.");

            var fileBytes = await _reporteService.ExportarCsvAsync(desde, hasta);
            return File(fileBytes, "text/csv", $"ReporteFinanciero_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
        }
    }
}
