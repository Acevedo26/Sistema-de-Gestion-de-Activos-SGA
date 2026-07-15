using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Reportes;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Controllers
{
	[ApiController]
	[Route("api/reportes/inventario")]
	[Authorize]
	public class ReportesInventarioController : ControllerBase
	{
		private readonly IReporteInventarioService _reporteService;

		public ReportesInventarioController(IReporteInventarioService reporteService)
		{
			_reporteService = reporteService;
		}

		// GET api/reportes/inventario — RF-12: Reporte de Inventario
		[HttpGet]
		public async Task<IActionResult> ObtenerReporteInventario(
			[FromQuery] Guid? categoriaId,
			[FromQuery] string? estado,
			[FromQuery] Guid? ubicacionId,
			[FromQuery] DateTime? fechaDesde,
			[FromQuery] DateTime? fechaHasta,
			[FromQuery] bool incluirDepreciacion = false)
		{
			var filtro = new ReporteInventarioFiltroDto
			{
				CategoriaId = categoriaId,
				Estado = estado,
				UbicacionId = ubicacionId,
				FechaDesde = fechaDesde,
				FechaHasta = fechaHasta,
				IncluirDepreciacion = incluirDepreciacion
			};

			var reporte = await _reporteService.GenerarReporteInventarioAsync(filtro);
			return Ok(reporte);
		}

		// GET api/reportes/inventario/exportar-csv — RF-12: exportación (Excel/CSV)
		[HttpGet("exportar-csv")]
		public async Task<IActionResult> ExportarCsv(
			[FromQuery] Guid? categoriaId,
			[FromQuery] string? estado,
			[FromQuery] Guid? ubicacionId,
			[FromQuery] DateTime? fechaDesde,
			[FromQuery] DateTime? fechaHasta,
			[FromQuery] bool incluirDepreciacion = false)
		{
			var filtro = new ReporteInventarioFiltroDto
			{
				CategoriaId = categoriaId,
				Estado = estado,
				UbicacionId = ubicacionId,
				FechaDesde = fechaDesde,
				FechaHasta = fechaHasta,
				IncluirDepreciacion = incluirDepreciacion
			};

			var fileBytes = await _reporteService.ExportarInventarioCsvAsync(filtro);
			return File(fileBytes, "text/csv", $"ReporteInventario_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
		}

		// GET api/reportes/inventario/exportar-excel — RF-12: exportación a Excel real
		[HttpGet("exportar-excel")]
		public async Task<IActionResult> ExportarExcel(
			[FromQuery] Guid? categoriaId,
			[FromQuery] string? estado,
			[FromQuery] Guid? ubicacionId,
			[FromQuery] DateTime? fechaDesde,
			[FromQuery] DateTime? fechaHasta,
			[FromQuery] bool incluirDepreciacion = false)
		{
			var filtro = new ReporteInventarioFiltroDto
			{
				CategoriaId = categoriaId,
				Estado = estado,
				UbicacionId = ubicacionId,
				FechaDesde = fechaDesde,
				FechaHasta = fechaHasta,
				IncluirDepreciacion = incluirDepreciacion
			};

			var fileBytes = await _reporteService.ExportarInventarioExcelAsync(filtro);
			return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
				$"ReporteInventario_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
		}

		// GET api/reportes/inventario/exportar-pdf — RF-12: exportación a PDF
		[HttpGet("exportar-pdf")]
		public async Task<IActionResult> ExportarPdf(
			[FromQuery] Guid? categoriaId,
			[FromQuery] string? estado,
			[FromQuery] Guid? ubicacionId,
			[FromQuery] DateTime? fechaDesde,
			[FromQuery] DateTime? fechaHasta,
			[FromQuery] bool incluirDepreciacion = false)
		{
			var filtro = new ReporteInventarioFiltroDto
			{
				CategoriaId = categoriaId,
				Estado = estado,
				UbicacionId = ubicacionId,
				FechaDesde = fechaDesde,
				FechaHasta = fechaHasta,
				IncluirDepreciacion = incluirDepreciacion
			};

			var fileBytes = await _reporteService.ExportarInventarioPdfAsync(filtro);
			return File(fileBytes, "application/pdf", $"ReporteInventario_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
		}

		// GET api/reportes/inventario/por-ubicacion — RF-14: Reporte de Activos por Ubicación
		[HttpGet("por-ubicacion")]
		public async Task<IActionResult> ObtenerReportePorUbicacion()
		{
			var reporte = await _reporteService.GenerarReportePorUbicacionAsync();
			return Ok(reporte);
		}
	}
}
