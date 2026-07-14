using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Sistema_de_Gestion_de_Activos.Domain.Entities;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Data.Interceptors
{
    public class AuditoriaInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditoriaInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, 
            InterceptionResult<int> result, 
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context == null) return result;

            var dbContext = eventData.Context;
            var entries = dbContext.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || 
                            e.State == EntityState.Modified || 
                            e.State == EntityState.Deleted)
                .ToList();

            var auditorias = new List<Auditoria>();

            // Obtener UsuarioId del token JWT si existe en el HttpContext actual
            Guid? usuarioId = null;
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out Guid parsedId))
            {
                usuarioId = parsedId;
            }

            // Lista de entidades a auditar explícitamente (RF-18: activos, mantenimientos)
            var entidadesAuditables = new[] { "Activo", "Mantenimiento" };

            foreach (var entry in entries)
            {
                var entityType = entry.Entity.GetType();
                if (!entidadesAuditables.Contains(entityType.Name)) continue;

                var auditoria = new Auditoria
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuarioId,
                    TablaAfectada = entityType.Name,
                    FechaAccion = DateTime.UtcNow
                };

                var (anteriores, nuevos) = ObtenerValores(entry);

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditoria.Accion = AccionAuditoria.INSERT;
                        auditoria.ValoresNuevos = JsonSerializer.Serialize(nuevos);
                        break;
                    case EntityState.Modified:
                        auditoria.Accion = AccionAuditoria.UPDATE;
                        auditoria.ValoresAnteriores = JsonSerializer.Serialize(anteriores);
                        auditoria.ValoresNuevos = JsonSerializer.Serialize(nuevos);
                        break;
                    case EntityState.Deleted:
                        auditoria.Accion = AccionAuditoria.DELETE;
                        auditoria.ValoresAnteriores = JsonSerializer.Serialize(anteriores);
                        break;
                }

                auditorias.Add(auditoria);
            }

            if (auditorias.Any())
            {
                await dbContext.Set<Auditoria>().AddRangeAsync(auditorias, cancellationToken);
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private (Dictionary<string, object> anteriores, Dictionary<string, object> nuevos) ObtenerValores(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            var anteriores = new Dictionary<string, object>();
            var nuevos = new Dictionary<string, object>();

            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary) continue; 

                string propName = property.Metadata.Name;

                switch (entry.State)
                {
                    case EntityState.Added:
                        nuevos[propName] = property.CurrentValue ?? string.Empty;
                        break;
                    case EntityState.Deleted:
                        anteriores[propName] = property.OriginalValue ?? string.Empty;
                        break;
                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            anteriores[propName] = property.OriginalValue ?? string.Empty;
                            nuevos[propName] = property.CurrentValue ?? string.Empty;
                        }
                        break;
                }
            }

            return (anteriores, nuevos);
        }
    }
}
