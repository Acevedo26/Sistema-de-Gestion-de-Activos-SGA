using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Depreciacion;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
    public interface IHistorialDepreciacionService
    {
        Task GenerarSnapshotMensualAsync();
        Task<IEnumerable<HistorialDepreciacionDto>> ObtenerHistorialPorActivoAsync(Guid activoId);
    }
}
