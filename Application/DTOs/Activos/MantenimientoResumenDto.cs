using System;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Activos
{
    public class MantenimientoResumenDto
    {
        public Guid Id { get; set; }
        public TipoMantenimiento Tipo { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string TecnicoNombre { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public decimal Costo { get; set; }
        public EstadoMantenimiento Estado { get; set; }
    }
}
