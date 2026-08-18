using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Gestion_de_Activos.Data;
using Sistema_de_Gestion_de_Activos.Domain.Entities;
using Sistema_de_Gestion_de_Activos.Domain.Enums;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class NotificacionDepreciacionService : INotificacionDepreciacionService
    {
        private readonly SgaDbContext _context;

        public NotificacionDepreciacionService(SgaDbContext context)
        {
            _context = context;
        }

        public async Task GenerarAlertasDepreciacionAsync()
        {
            var candidatas = await _context.Depreciaciones
                .Include(d => d.Activo)
                .Where(d => d.PorcentajeConsumido >= 90m && d.Activo.Estado != EstadoActivo.Baja && d.Activo.Estado != EstadoActivo.Descartado)
                .ToListAsync();

            foreach (var dep in candidatas)
            {
                var existe = await _context.Notificaciones.AnyAsync(n => n.Tipo == TipoNotificacion.DepreciacionProxima && n.ActivoId == dep.ActivoId && !n.Leida);
                if (existe) continue;
                _context.Notificaciones.Add(new Notificacion
                {
                    Id = Guid.NewGuid(),
                    Tipo = TipoNotificacion.DepreciacionProxima,
                    ActivoId = dep.ActivoId,
                    Mensaje = $"El activo {dep.Activo.Codigo} alcanzó {dep.PorcentajeConsumido:N1}% de depreciación.",
                    FechaGeneracion = DateTime.UtcNow
                });
            }
            await _context.SaveChangesAsync();
        }
    }
}
