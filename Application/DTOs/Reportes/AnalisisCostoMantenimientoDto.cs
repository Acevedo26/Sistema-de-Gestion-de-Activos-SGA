using System;
using System.Collections.Generic;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Reportes
{
	/// RF-15: Análisis de Costos de Mantenimiento (por activo, categoría o período).
	public class AnalisisCostoActivoDto
	{
		public Guid ActivoId { get; set; }
		public string Codigo { get; set; } = string.Empty;
		public string Descripcion { get; set; } = string.Empty;
		public string Categoria { get; set; } = string.Empty;
		public int CantidadMantenimientos { get; set; }
		public decimal CostoTotalMantenimiento { get; set; }
		public decimal ValorActual { get; set; }
		public decimal RelacionCostoValor { get; set; }
		public bool CostoExcesivo { get; set; }
	}

	public class AnalisisCostoCategoriaDto
	{
		public Guid CategoriaId { get; set; }
		public string Categoria { get; set; } = string.Empty;
		public int CantidadActivos { get; set; }
		public decimal CostoTotalMantenimiento { get; set; }
		public decimal CostoPromedioPorActivo { get; set; }
	}

	public class AnalisisCostoMantenimientoResultDto
	{
		public List<AnalisisCostoActivoDto> PorActivo { get; set; } = new();
		public List<AnalisisCostoCategoriaDto> PorCategoria { get; set; } = new();
		public decimal UmbralCostoExcesivo { get; set; }
	}
}