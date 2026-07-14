using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Depreciacion;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Data;
using Sistema_de_Gestion_de_Activos.Domain.Entities;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class HistorialDepreciacionService : IHistorialDepreciacionService
    {
        private readonly SgaDbContext _context;
        private readonly IDepreciacionService _depreciacionService;

        public HistorialDepreciacionService(SgaDbContext context, IDepreciacionService depreciacionService)
        {
            _context = context;
            _depreciacionService = depreciacionService;
        }

        public async Task GenerarSnapshotMensualAsync()
        {
            // Primero aseguramos que todo esté recalculado a la fecha de hoy
            await _depreciacionService.RecalcularTodosAsync();

            var depreciaciones = await _context.Depreciaciones
                .Include(d => d.Activo)
                .Where(d => d.Activo.Estado != Domain.Enums.EstadoActivo.Baja && d.Activo.Estado != Domain.Enums.EstadoActivo.Descartado)
                .ToListAsync();

            var fechaCorte = DateTime.UtcNow;

            foreach (var dep in depreciaciones)
            {
                var snapshot = new HistorialDepreciacion
                {
                    Id = Guid.NewGuid(),
                    DepreciacionId = dep.Id,
                    FechaConsulta = fechaCorte,
                    ValorActual = dep.ValorActual,
                    ValorResidual = dep.ValorResidual,
                    PorcentajeConsumido = dep.PorcentajeConsumido,
                    DepreciacionAcumulada = Math.Max(0, dep.Activo.CostoInicial - dep.ValorActual)
                };
                
                await _context.HistorialDepreciaciones.AddAsync(snapshot);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<HistorialDepreciacionDto>> ObtenerHistorialPorActivoAsync(Guid activoId)
        {
            var historial = await _context.HistorialDepreciaciones
                .Include(h => h.Depreciacion)
                .Where(h => h.Depreciacion.ActivoId == activoId)
                .OrderByDescending(h => h.FechaConsulta)
                .Select(h => new HistorialDepreciacionDto
                {
                    Id = h.Id,
                    FechaConsulta = h.FechaConsulta,
                    ValorActual = h.ValorActual,
                    ValorResidual = h.ValorResidual,
                    PorcentajeConsumido = h.PorcentajeConsumido,
                    DepreciacionAcumulada = h.DepreciacionAcumulada
                })
                .ToListAsync();

            return historial;
        }
    }
}
