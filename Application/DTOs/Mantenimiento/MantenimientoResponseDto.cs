using System;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Mantenimiento
{
	public class MantenimientoResponseDto
	{
		public Guid Id { get; set; }
		public Guid ActivoId { get; set; }
		public string ActivoDescripcion { get; set; } = string.Empty;
		public Guid TecnicoId { get; set; }
		public string TecnicoNombre { get; set; } = string.Empty;
		public TipoMantenimiento Tipo { get; set; }
		public EstadoMantenimiento Estado { get; set; }
		public DateTime FechaInicio { get; set; }
		public DateTime? FechaFin { get; set; }
		public decimal Costo { get; set; }
		public string Descripcion { get; set; } = string.Empty;
		public string? Observaciones { get; set; }
	}
}