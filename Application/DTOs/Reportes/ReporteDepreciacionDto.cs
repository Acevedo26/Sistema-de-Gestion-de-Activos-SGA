using System;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Reportes
{
    public class ReporteDepreciacionDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal CostoOriginal { get; set; }
        public decimal ValorActual { get; set; }
        public decimal ValorResidual { get; set; }
        public decimal DepreciacionAcumulada { get; set; }
        public decimal PorcentajeConsumido { get; set; }
        public int AniosVidaUtilRestantes { get; set; }
        public DateTime FechaEstimadaFinDepreciacion { get; set; }
    }
}
