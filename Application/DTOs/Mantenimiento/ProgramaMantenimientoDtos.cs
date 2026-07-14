using System;
using System.ComponentModel.DataAnnotations;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Mantenimiento
{
	public class ProgramaMantenimientoResponseDto
	{
		public Guid Id { get; set; }
		public Guid ActivoId { get; set; }
		public string ActivoDescripcion { get; set; } = string.Empty;
		public string Titulo { get; set; } = string.Empty;
		public int FrecuenciaDias { get; set; }
		public DateTime ProximaFecha { get; set; }
		public EstadoPrograma Estado { get; set; }
		public Guid? TecnicoAsignadoId { get; set; }
		public string? TecnicoAsignadoNombre { get; set; }
	}

	public class ProgramaMantenimientoCreateDto
	{
		[Required(ErrorMessage = "El activo es obligatorio.")]
		public Guid ActivoId { get; set; }

		[Required(ErrorMessage = "El título es obligatorio.")]
		[MaxLength(150)]
		public string Titulo { get; set; } = string.Empty;

		[Required(ErrorMessage = "La frecuencia es obligatoria.")]
		[Range(1, 3650, ErrorMessage = "La frecuencia debe estar entre 1 y 3650 días.")]
		public int FrecuenciaDias { get; set; }

		[Required(ErrorMessage = "La próxima fecha es obligatoria.")]
		public DateTime ProximaFecha { get; set; }

		public Guid? TecnicoAsignadoId { get; set; }
	}
}