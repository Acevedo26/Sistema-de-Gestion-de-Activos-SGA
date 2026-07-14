using System;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class AnalisisIntegradoService : IAnalisisIntegradoService
    {
        public Task<decimal> ObtenerRelacionCostoValorAsync(Guid activoId)
        {
            return Task.FromResult(0m);
        }
    }
}
