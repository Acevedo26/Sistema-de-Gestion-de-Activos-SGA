using System.Collections.Generic;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Dashboard;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardKpiDto> GetKPIsAsync();
        Task<List<AlertaMantenimientoDto>> GetAlertasMantenimientoAsync();
        Task<List<ActividadRecienteDto>> GetActividadRecienteAsync();
    }
}