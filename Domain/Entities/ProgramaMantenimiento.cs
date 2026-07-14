using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
	[Table("programas_mantenimiento")]
	public class ProgramaMantenimiento
	{
		[Key]
		[Column("id")]
		public Guid Id { get; set; }

		[Required]
		[Column("activo_id")]
		public Guid ActivoId { get; set; }

		[Required]
		[MaxLength(150)]
		[Column("titulo")]
		public string Titulo { get; set; } = string.Empty;

		[Required]
		[Column("frecuencia_dias")]
		public int FrecuenciaDias { get; set; }

		[Required]
		[Column("proxima_fecha")]
		public DateTime ProximaFecha { get; set; }

		[Required]
		[Column("estado")]
		public EstadoPrograma Estado { get; set; } = EstadoPrograma.Activo;

		[Column("tecnico_asignado_id")]
		public Guid? TecnicoAsignadoId { get; set; }

		// Navegación
		[ForeignKey(nameof(ActivoId))]
		public Activo Activo { get; set; } = null!;

		[ForeignKey(nameof(TecnicoAsignadoId))]
		public Usuario? TecnicoAsignado { get; set; }
	}
}