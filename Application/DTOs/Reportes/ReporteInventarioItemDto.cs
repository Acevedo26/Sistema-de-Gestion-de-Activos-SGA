using System;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Reportes
{
	/// RF-12: Reporte de Inventario.
	public class ReporteInventarioItemDto
	{
		public Guid Id { get; set; }
		public string Codigo { get; set; } = string.Empty;
		public string Descripcion { get; set; } = string.Empty;
		public string Categoria { get; set; } = string.Empty;
		public string Estado { get; set; } = string.Empty;
		public string Ubicacion { get; set; } = string.Empty;
		public string Responsable { get; set; } = "Sin asignar";
		public DateTime FechaAdquisicion { get; set; }
		public decimal Valor { get; set; }

		// Columnas opcionales de depreciación (RF-12: "opción de incluir columnas de depreciación")
		public decimal? ValorActual { get; set; }
		public decimal? PorcentajeConsumido { get; set; }
	}

	public class ReporteInventarioFiltroDto
	{
		public Guid? CategoriaId { get; set; }
		public string? Estado { get; set; }
		public Guid? UbicacionId { get; set; }
		public DateTime? FechaDesde { get; set; }
		public DateTime? FechaHasta { get; set; }
		public bool IncluirDepreciacion { get; set; } = false;
	}
}