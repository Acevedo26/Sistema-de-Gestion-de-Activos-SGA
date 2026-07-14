using System;
using System.Threading.Tasks;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
    public interface IAnalisisIntegradoService
    {
        Task<decimal> ObtenerRelacionCostoValorAsync(Guid activoId);
    }
}
