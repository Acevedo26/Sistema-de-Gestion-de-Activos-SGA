using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Reportes;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Controllers
{
	[ApiController]
	[Route("api/reportes/mantenimientos")]
	[Authorize]
	public class ReportesMantenimientoController : ControllerBase
	{
		private readonly IReporteMantenimientoAnalisisService _reporteService;

		public ReportesMantenimientoController(IReporteMantenimientoAnalisisService reporteService)
		{
			_reporteService = reporteService;
		}

		// GET api/reportes/mantenimientos?desde=...&hasta=... — RF-13: Reporte de Mantenimientos
		[HttpGet]
		public async Task<IActionResult> ObtenerReporteMantenimientos(
			[FromQuery] DateTime desde,
			[FromQuery] DateTime hasta,
			[FromQuery] Guid? categoriaId,
			[FromQuery] Guid? tecnicoId)
		{
			if (desde == default || hasta == default || desde > hasta)
				return BadRequest("Rango de fechas inválido.");

			var filtro = new ReporteMantenimientoFiltroDto
			{
				Desde = desde,
				Hasta = hasta,
				CategoriaId = categoriaId,
				TecnicoId = tecnicoId
			};

			var reporte = await _reporteService.GenerarReporteMantenimientosAsync(filtro);
			return Ok(reporte);
		}

		// GET api/reportes/mantenimientos/exportar-csv — RF-13: exportación (Excel/CSV)
		[HttpGet("exportar-csv")]
		public async Task<IActionResult> ExportarCsv(
			[FromQuery] DateTime desde,
			[FromQuery] DateTime hasta,
			[FromQuery] Guid? categoriaId,
			[FromQuery] Guid? tecnicoId)
		{
			if (desde == default || hasta == default || desde > hasta)
				return BadRequest("Rango de fechas inválido.");

			var filtro = new ReporteMantenimientoFiltroDto
			{
				Desde = desde,
				Hasta = hasta,
				CategoriaId = categoriaId,
				TecnicoId = tecnicoId
			};

			var fileBytes = await _reporteService.ExportarMantenimientosCsvAsync(filtro);
			return File(fileBytes, "text/csv", $"ReporteMantenimientos_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
		}

		// GET api/reportes/mantenimientos/exportar-excel — RF-13: exportación a Excel real
		[HttpGet("exportar-excel")]
		public async Task<IActionResult> ExportarExcel(
			[FromQuery] DateTime desde,
			[FromQuery] DateTime hasta,
			[FromQuery] Guid? categoriaId,
			[FromQuery] Guid? tecnicoId)
		{
			if (desde == default || hasta == default || desde > hasta)
				return BadRequest("Rango de fechas inválido.");

			var filtro = new ReporteMantenimientoFiltroDto
			{
				Desde = desde,
				Hasta = hasta,
				CategoriaId = categoriaId,
				TecnicoId = tecnicoId
			};

			var fileBytes = await _reporteService.ExportarMantenimientosExcelAsync(filtro);
			return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
				$"ReporteMantenimientos_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
		}

		// GET api/reportes/mantenimientos/exportar-pdf — RF-13: exportación a PDF
		[HttpGet("exportar-pdf")]
		public async Task<IActionResult> ExportarPdf(
			[FromQuery] DateTime desde,
			[FromQuery] DateTime hasta,
			[FromQuery] Guid? categoriaId,
			[FromQuery] Guid? tecnicoId)
		{
			if (desde == default || hasta == default || desde > hasta)
				return BadRequest("Rango de fechas inválido.");

			var filtro = new ReporteMantenimientoFiltroDto
			{
				Hasta = hasta,
				CategoriaId = categoriaId,
				TecnicoId = tecnicoId
			};

			var fileBytes = await _reporteService.ExportarMantenimientosPdfAsync(filtro);
			return File(fileBytes, "application/pdf", $"ReporteMantenimientos_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
		}

		// GET api/reportes/mantenimientos/analisis-costos — RF-15: Análisis de Costos de Mantenimiento
		[HttpGet("analisis-costos")]
		public async Task<IActionResult> ObtenerAnalisisCostos(
			[FromQuery] Guid? categoriaId,
			[FromQuery] DateTime? desde,
			[FromQuery] DateTime? hasta)
		{
			var analisis = await _reporteService.GenerarAnalisisCostosAsync(categoriaId, desde, hasta);
			return Ok(analisis);
		}
	}
}
