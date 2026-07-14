using System;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Depreciacion
{
    public class DepreciacionDto
    {
        public decimal ValorOriginal { get; set; }
        public decimal ValorActual { get; set; }
        public decimal ValorResidual { get; set; }
        public decimal PorcentajeConsumido { get; set; }
        public int AntiguedadAnios { get; set; }
        public int AntiguedadMeses { get; set; }
        public bool EsCompletamenteDepreciado => PorcentajeConsumido >= 100m;
        public int VidaUtilAsignada { get; set; }
        public DateTime FechaEstimadaFinDepreciacion { get; set; }
    }
}
