using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Sistema_de_Gestion_de_Activos.Domain.Entities;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Data.Interceptors
{
    public class AuditoriaInterceptor : SaveChangesInterceptor
    {
        private readonly IServiceProvider _serviceProvider;

        public AuditoriaInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, 
            InterceptionResult<int> result, 
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context == null) return result;

            var context = eventData.Context;
            var entries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || 
                            e.State == EntityState.Modified || 
                            e.State == EntityState.Deleted)
                .ToList();

            if (!entries.Any()) return result;

            var auditorias = new List<Auditoria>();

            Guid? usuarioId = null;
            try
            {
                var authStateProvider = _serviceProvider.GetService<AuthenticationStateProvider>();
                if (authStateProvider != null)
                {
                    var authState = await authStateProvider.GetAuthenticationStateAsync();
                    var userIdClaim = authState.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (Guid.TryParse(userIdClaim, out Guid parsedId))
                    {
                        usuarioId = parsedId;
                    }
                }
                else
                {
                    var httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
                    var userIdClaim = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (Guid.TryParse(userIdClaim, out Guid parsedId))
                    {
                        usuarioId = parsedId;
                    }
                }
            }
            catch
            {
                // Ignorar errores al obtener usuario
            }

            // Lista de entidades a auditar explícitamente (RF-18: activos, mantenimientos)
            var entidadesAuditables = new[]
            {
                "Activo", "Mantenimiento", "Ubicacion", "Usuario", "CategoriaVidaUtil", "Movimiento"
            };

            foreach (var entry in entries)
            {
                var entityType = entry.Entity.GetType();
                
                // Si la entidad es proxy, obtener el tipo base
                var typeName = entityType.Name;
                if (typeName.Contains("Proxy"))
                {
                    typeName = entityType.BaseType?.Name ?? typeName;
                }

                // Filtrar solo las entidades que queremos auditar para evitar recursión o logs basura
                if (!entidadesAuditables.Contains(typeName) && !entidadesAuditables.Any(e => typeName.StartsWith(e)))
                    continue;

                var auditoria = new Auditoria
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuarioId,
                    TablaAfectada = typeName,
                    FechaAccion = DateTime.UtcNow
                };

                var (anteriores, nuevos) = ObtenerValores(entry);

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditoria.Accion = AccionAuditoria.INSERT;
                        auditoria.ValoresNuevos = nuevos;
                        break;
                    case EntityState.Modified:
                        auditoria.Accion = AccionAuditoria.UPDATE;
                        auditoria.ValoresAnteriores = anteriores;
                        auditoria.ValoresNuevos = nuevos;
                        break;
                    case EntityState.Deleted:
                        auditoria.Accion = AccionAuditoria.DELETE;
                        auditoria.ValoresAnteriores = anteriores;
                        break;
                }

                auditorias.Add(auditoria);
            }

            if (auditorias.Any())
            {
                await context.Set<Auditoria>().AddRangeAsync(auditorias, cancellationToken);
            }

            return result;
        }

        private (string? Anteriores, string? Nuevos) ObtenerValores(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            var anteriores = new Dictionary<string, object?>();
            var nuevos = new Dictionary<string, object?>();

            foreach (var property in entry.Properties)
            {
                // Ignorar propiedades de navegación para evitar serialización infinita
                if (property.Metadata.IsPrimaryKey()) continue;

                var propName = property.Metadata.Name;

                switch (entry.State)
                {
                    case EntityState.Added:
                        nuevos[propName] = property.CurrentValue;
                        break;
                    case EntityState.Deleted:
                        anteriores[propName] = property.OriginalValue;
                        break;
                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            var originalValue = property.OriginalValue;
                            var currentValue = property.CurrentValue;
                            
                            // Solo registrar si realmente cambió
                            if (!Equals(originalValue, currentValue))
                            {
                                anteriores[propName] = originalValue;
                                nuevos[propName] = currentValue;
                            }
                        }
                        break;
                }
            }

            var opciones = new JsonSerializerOptions { WriteIndented = false };
            
            return (
                anteriores.Any() ? JsonSerializer.Serialize(anteriores, opciones) : null,
                nuevos.Any() ? JsonSerializer.Serialize(nuevos, opciones) : null
            );
        }
    }
}