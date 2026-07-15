using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Reportes;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
	public interface IReporteMantenimientoAnalisisService
	{
		// RF-13: Reporte de Mantenimientos
		Task<IEnumerable<ReporteMantenimientoItemDto>> GenerarReporteMantenimientosAsync(ReporteMantenimientoFiltroDto filtro);

		// RF-13: exportación a CSV (compatible con Excel)
		Task<byte[]> ExportarMantenimientosCsvAsync(ReporteMantenimientoFiltroDto filtro);

		// RF-13: exportación a Excel real (.xlsx)
		Task<byte[]> ExportarMantenimientosExcelAsync(ReporteMantenimientoFiltroDto filtro);

		// RF-13: exportación a PDF
		Task<byte[]> ExportarMantenimientosPdfAsync(ReporteMantenimientoFiltroDto filtro);

		// RF-15: Análisis de Costos de Mantenimiento
		Task<AnalisisCostoMantenimientoResultDto> GenerarAnalisisCostosAsync(Guid? categoriaId, DateTime? desde, DateTime? hasta);
	}
}
