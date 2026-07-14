using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Domain.Entities
{
	[Table("mantenimientos")]
	public class Mantenimiento
	{
		[Key]
		[Column("id")]
		public Guid Id { get; set; }

		[Required]
		[Column("activo_id")]
		public Guid ActivoId { get; set; }

		[Required]
		[Column("tecnico_id")]
		public Guid TecnicoId { get; set; }

		[Required]
		[Column("tipo")]
		public TipoMantenimiento Tipo { get; set; }

		[Required]
		[Column("estado")]
		public EstadoMantenimiento Estado { get; set; } = EstadoMantenimiento.Pendiente;

		[Required]
		[Column("fecha_inicio")]
		public DateTime FechaInicio { get; set; }

		[Column("fecha_fin")]
		public DateTime? FechaFin { get; set; }

		[Required]
		[Column("costo", TypeName = "decimal(18,2)")]
		public decimal Costo { get; set; } = 0m;

		[Required]
		[MaxLength(500)]
		[Column("descripcion")]
		public string Descripcion { get; set; } = string.Empty;

		[MaxLength(500)]
		[Column("observaciones")]
		public string? Observaciones { get; set; }

		// Navegación
		[ForeignKey(nameof(ActivoId))]
		public Activo Activo { get; set; } = null!;

		[ForeignKey(nameof(TecnicoId))]
		public Usuario Tecnico { get; set; } = null!;
	}
}