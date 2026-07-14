using System;
using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Mantenimiento
{
	public class MantenimientoFinalizarDto
	{
		[Required(ErrorMessage = "La fecha de finalización es obligatoria.")]
		public DateTime FechaFin { get; set; }

		[Range(0, double.MaxValue, ErrorMessage = "El costo no puede ser negativo.")]
		public decimal Costo { get; set; }
	}
}