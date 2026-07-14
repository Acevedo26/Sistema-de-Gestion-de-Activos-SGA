namespace Sistema_de_Gestion_de_Activos.Application.DTOs.Reportes
{
    public class AnalisisValorDepartamentoDto
    {
        public string Departamento { get; set; } = string.Empty;
        public string Oficina { get; set; } = string.Empty;
        public decimal ValorOriginalTotal { get; set; }
        public decimal ValorActualTotal { get; set; }
        public decimal DepreciacionTotalAcumulada { get; set; }
        public decimal PromedioAntiguedadAnios { get; set; }
    }
}
