using System;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Reportes
{
	/// RF-13: Reporte de Mantenimientos.
	public class ReporteMantenimientoItemDto
	{
		public Guid MantenimientoId { get; set; }
		public string CodigoActivo { get; set; } = string.Empty;
		public string DescripcionActivo { get; set; } = string.Empty;
		public string Categoria { get; set; } = string.Empty;
		public string Tipo { get; set; } = string.Empty;
		public string Tecnico { get; set; } = string.Empty;
		public DateTime FechaInicio { get; set; }
		public DateTime? FechaFin { get; set; }
		public decimal Costo { get; set; }
		public string Estado { get; set; } = string.Empty;

		// "Incluir análisis de costo de mantenimiento vs. valor actual del activo" (RF-13)
		public decimal ValorActualActivo { get; set; }
		public decimal RelacionCostoValor { get; set; }
	}

	public class ReporteMantenimientoFiltroDto
	{
		public DateTime Desde { get; set; }
		public DateTime Hasta { get; set; }
		public Guid? CategoriaId { get; set; }
		public Guid? TecnicoId { get; set; }
	}
}