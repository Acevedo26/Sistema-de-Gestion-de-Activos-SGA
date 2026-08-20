namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Activos
{
    public class AnalisisRendimientoDto
    {
        public decimal CostoTotalMantenimiento { get; set; }
        public decimal ValorActual { get; set; }
        public decimal PorcentajeCostoVsValor { get; set; }
        public bool AlertaCostoExcesivo { get; set; }
        public string Recomendacion { get; set; } = string.Empty;
    }
}
