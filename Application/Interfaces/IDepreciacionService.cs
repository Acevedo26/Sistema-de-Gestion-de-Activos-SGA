using System;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Depreciacion;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
    public interface IDepreciacionService
    {
        Task InicializarDepreciacionAsync(Guid activoId, decimal costoInicial, Guid categoriaId);
        Task RecalcularAsync(Guid activoId);
        Task RecalcularTodosAsync();
        Task<DepreciacionDto?> ObtenerDetalleFinancieroAsync(Guid activoId);
    }
}
