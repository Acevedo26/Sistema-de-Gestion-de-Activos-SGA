using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Maestros;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Domain.Entities;
using Sistema_de_Gestion_de_Activos.Repositories.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Notificacion> _repository;

        public NotificationService(IRepository<Notificacion> repository)
        {
            _repository = repository;
        }

        public async Task<List<NotificacionDto>> ObtenerRecientesAsync()
        {
            var notificaciones = await _repository.GetAllAsync();
            return notificaciones.OrderByDescending(n => n.FechaGeneracion)
                .Take(50)
                .Select(n => new NotificacionDto
                {
                    Id = n.Id,
                    Titulo = n.Tipo.ToString(),
                    Mensaje = n.Mensaje,
                    FechaGeneracion = n.FechaGeneracion,
                    Leida = n.Leida
                }).ToList();
        }

        public async Task<bool> MarcarComoLeidaAsync(Guid id)
        {
            var notificacion = await _repository.GetByIdAsync(id);
            if (notificacion == null)
            {
                return false;
            }

            notificacion.Leida = true;
            _repository.Update(notificacion);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> MarcarTodasComoLeidasAsync()
        {
            var noLeidas = await _repository.FindAsync(n => !n.Leida);
            foreach (var notificacion in noLeidas)
            {
                notificacion.Leida = true;
                _repository.Update(notificacion);
            }
            return await _repository.SaveChangesAsync();
        }
    }
}
