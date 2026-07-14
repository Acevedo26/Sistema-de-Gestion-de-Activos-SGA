using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class NotificacionDepreciacionService : INotificacionDepreciacionService
    {
        public Task GenerarAlertasDepreciacionAsync()
        {
            // Stub para integración con alertas
            return Task.CompletedTask;
        }
    }
}
