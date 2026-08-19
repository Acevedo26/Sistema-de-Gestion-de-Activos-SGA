using System;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Dashboard
{
    public class DashboardKpiDto
    {
        public int TotalActivos { get; set; }
        public int ActivosNuevosMes { get; set; }
        public int ActivosDepreciados { get; set; }
        public double PorcentajeDepreciados => TotalActivos == 0 ? 0 : (ActivosDepreciados * 100.0) / TotalActivos;
        public decimal ValorActualTotal { get; set; }
        public decimal DepreciacionAcumuladaTotal { get; set; }
        public int MantenimientosProximos { get; set; }
    }

    public class AlertaMantenimientoDto
    {
        public Guid ActivoId { get; set; }
        public string NombreActivo { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
        public int DiasRestantes => (FechaVencimiento.Date - DateTime.Now.Date).Days;
        public string EstadoUrgencia => DiasRestantes <= 0 ? "Rojo" : DiasRestantes <= 3 ? "Amarillo" : "Verde";
    }

    public class ActividadRecienteDto
    {
        public Guid Id { get; set; }
        public string TipoEvento { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string TiempoRelativo { get; set; } = string.Empty;
        public DateTime FechaOriginal { get; set; }
    }
}