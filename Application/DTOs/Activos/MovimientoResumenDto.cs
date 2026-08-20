using System;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Activos
{
    public class MovimientoResumenDto
    {
        public Guid Id { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string UsuarioOperador { get; set; } = string.Empty;
        public string UbicacionAnterior { get; set; } = string.Empty;
        public string UbicacionNueva { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
    }
}
