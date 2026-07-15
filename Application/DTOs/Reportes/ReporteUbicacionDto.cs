using System;
using System.Collections.Generic;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Reportes
{
	/// RF-14: Reporte de Activos por Ubicación.
	public class ReporteUbicacionDto
	{
		public Guid UbicacionId { get; set; }
		public string Edificio { get; set; } = string.Empty;
		public string Departamento { get; set; } = string.Empty;
		public string Oficina { get; set; } = string.Empty;
		public int CantidadActivos { get; set; }
		public decimal ValorTotal { get; set; }
		public List<ActivoResumenDto> Activos { get; set; } = new();
	}

	public class ActivoResumenDto
	{
		public Guid Id { get; set; }
		public string Codigo { get; set; } = string.Empty;
		public string Descripcion { get; set; } = string.Empty;
		public string Estado { get; set; } = string.Empty;
	}
}