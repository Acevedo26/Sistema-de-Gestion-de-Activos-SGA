using System;
using System.ComponentModel.DataAnnotations;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Mantenimiento
{
	public class MantenimientoCreateDto
	{
		[Required(ErrorMessage = "El activo es obligatorio.")]
		public Guid ActivoId { get; set; }

		[Required(ErrorMessage = "El técnico responsable es obligatorio.")]
		public Guid TecnicoId { get; set; }

		[Required(ErrorMessage = "El tipo de mantenimiento es obligatorio.")]
		public TipoMantenimiento Tipo { get; set; }

		[Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
		public DateTime FechaInicio { get; set; }

		public DateTime? FechaFin { get; set; }

		[Range(0, double.MaxValue, ErrorMessage = "El costo no puede ser negativo.")]
		public decimal Costo { get; set; } = 0m;

		[Required(ErrorMessage = "La descripción es obligatoria.")]
		[MaxLength(500)]
		public string Descripcion { get; set; } = string.Empty;

		[MaxLength(500)]
		public string? Observaciones { get; set; }
	}
}