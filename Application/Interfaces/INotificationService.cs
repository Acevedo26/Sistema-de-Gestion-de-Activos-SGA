using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Maestros;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificacionDto>> ObtenerRecientesAsync();
        Task<bool> MarcarComoLeidaAsync(Guid id);
        Task<bool> MarcarTodasComoLeidasAsync();
    }
}
