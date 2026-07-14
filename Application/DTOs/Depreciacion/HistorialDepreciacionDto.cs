using System;

namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Depreciacion
{
    public class HistorialDepreciacionDto
    {
        public Guid Id { get; set; }
        public DateTime FechaConsulta { get; set; }
        public decimal ValorActual { get; set; }
        public decimal ValorResidual { get; set; }
        public decimal PorcentajeConsumido { get; set; }
        public decimal DepreciacionAcumulada { get; set; }
    }
}
