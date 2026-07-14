using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Reportes;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
    public interface IReporteDepreciacionService
    {
        Task<IEnumerable<ReporteDepreciacionDto>> GenerarReporteGeneralAsync(Guid? categoriaId, string? estadoDepreciacion);
        Task<IEnumerable<AnalisisValorDepartamentoDto>> GenerarAnalisisPorDepartamentoAsync(Guid? ubicacionId);
        Task<byte[]> ExportarCsvAsync(DateTime desde, DateTime hasta);
    }
}
