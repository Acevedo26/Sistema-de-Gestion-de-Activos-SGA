using System.Collections.Generic;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Auditoria;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
    public interface IAuditoriaService
    {
        Task<IEnumerable<AuditoriaResponseDto>> ObtenerTodasAsync();
    }
}
