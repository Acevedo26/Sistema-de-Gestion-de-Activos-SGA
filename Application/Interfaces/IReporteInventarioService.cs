using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Reportes;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
	public interface IReporteInventarioService
	{
		// RF-12: Reporte de Inventario
		Task<IEnumerable<ReporteInventarioItemDto>> GenerarReporteInventarioAsync(ReporteInventarioFiltroDto filtro);

		// RF-12: exportación a CSV (compatible con Excel)
		Task<byte[]> ExportarInventarioCsvAsync(ReporteInventarioFiltroDto filtro);

		// RF-12: exportación a Excel real (.xlsx)
		Task<byte[]> ExportarInventarioExcelAsync(ReporteInventarioFiltroDto filtro);

		// RF-12: exportación a PDF
		Task<byte[]> ExportarInventarioPdfAsync(ReporteInventarioFiltroDto filtro);

		// RF-14: Reporte de Activos por Ubicación
		Task<IEnumerable<ReporteUbicacionDto>> GenerarReportePorUbicacionAsync();
	}
}
