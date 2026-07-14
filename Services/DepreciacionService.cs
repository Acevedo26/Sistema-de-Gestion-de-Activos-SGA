using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Depreciacion;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Data;
using Sistema_de_Gestion_de_Activos.Domain.Entities;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class DepreciacionService : IDepreciacionService
    {
        private readonly SgaDbContext _context;

        public DepreciacionService(SgaDbContext context)
        {
            _context = context;
        }

        public async Task InicializarDepreciacionAsync(Guid activoId, decimal costoInicial, Guid categoriaId)
        {
            var categoria = await _context.Categorias.FindAsync(categoriaId);
            if (categoria == null) throw new Exception("Categoría no encontrada.");

            var depreciacion = new Depreciacion
            {
                Id = Guid.NewGuid(),
                ActivoId = activoId,
                VidaUtilAsignada = categoria.VidaUtil,
                ValorResidual = categoria.ValorResidual,
                ValorActual = costoInicial,
                PorcentajeConsumido = 0m,
                FechaUltimoCalculo = DateTime.UtcNow
            };

            await _context.Depreciaciones.AddAsync(depreciacion);
            await _context.SaveChangesAsync();
        }

        public async Task RecalcularAsync(Guid activoId)
        {
            var activo = await _context.Activos
                .Include(a => a.Depreciacion)
                .FirstOrDefaultAsync(a => a.Id == activoId);

            if (activo == null || activo.Depreciacion == null) return;

            var dep = activo.Depreciacion;
            var costoInicial = activo.CostoInicial;
            var valorResidual = dep.ValorResidual;
            var vidaUtilAsignada = dep.VidaUtilAsignada;

            var tiempoTranscurrido = (decimal)(DateTime.UtcNow - activo.FechaAdquisicion).TotalDays / 365.25m;
            
            // Si la vida util asignada es 0 o menor, se considera ya depreciado
            if (vidaUtilAsignada <= 0)
            {
                dep.ValorActual = valorResidual;
                dep.PorcentajeConsumido = 100m;
            }
            else
            {
                var depreciacionAnual = (costoInicial - valorResidual) / vidaUtilAsignada;
                var depreciacionAcumulada = Math.Min(depreciacionAnual * tiempoTranscurrido, costoInicial - valorResidual);
                
                // Asegurar que nunca baje del valor residual (RF-23)
                dep.ValorActual = Math.Max(costoInicial - depreciacionAcumulada, valorResidual);
                dep.PorcentajeConsumido = Math.Min((tiempoTranscurrido / vidaUtilAsignada) * 100m, 100m);
            }

            dep.FechaUltimoCalculo = DateTime.UtcNow;
            
            _context.Depreciaciones.Update(dep);
            await _context.SaveChangesAsync();
        }

        public async Task RecalcularTodosAsync()
        {
            var activos = await _context.Activos
                .Include(a => a.Depreciacion)
                .Where(a => a.Depreciacion != null && a.Depreciacion.PorcentajeConsumido < 100m)
                .ToListAsync();

            foreach (var activo in activos)
            {
                var dep = activo.Depreciacion!;
                var costoInicial = activo.CostoInicial;
                var valorResidual = dep.ValorResidual;
                var vidaUtilAsignada = dep.VidaUtilAsignada;

                var tiempoTranscurrido = (decimal)(DateTime.UtcNow - activo.FechaAdquisicion).TotalDays / 365.25m;

                if (vidaUtilAsignada <= 0)
                {
                    dep.ValorActual = valorResidual;
                    dep.PorcentajeConsumido = 100m;
                }
                else
                {
                    var depreciacionAnual = (costoInicial - valorResidual) / vidaUtilAsignada;
                    var depreciacionAcumulada = Math.Min(depreciacionAnual * tiempoTranscurrido, costoInicial - valorResidual);
                    dep.ValorActual = Math.Max(costoInicial - depreciacionAcumulada, valorResidual);
                    dep.PorcentajeConsumido = Math.Min((tiempoTranscurrido / vidaUtilAsignada) * 100m, 100m);
                }

                dep.FechaUltimoCalculo = DateTime.UtcNow;
                _context.Depreciaciones.Update(dep);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<DepreciacionDto?> ObtenerDetalleFinancieroAsync(Guid activoId)
        {
            var activo = await _context.Activos
                .Include(a => a.Depreciacion)
                .FirstOrDefaultAsync(a => a.Id == activoId);

            if (activo == null || activo.Depreciacion == null) return null;

            var dep = activo.Depreciacion;
            var tiempo = DateTime.UtcNow - activo.FechaAdquisicion;
            var anios = (int)(tiempo.TotalDays / 365.25);
            var meses = (int)((tiempo.TotalDays % 365.25) / 30.44);

            return new DepreciacionDto
            {
                ValorOriginal = activo.CostoInicial,
                ValorActual = dep.ValorActual,
                ValorResidual = dep.ValorResidual,
                PorcentajeConsumido = dep.PorcentajeConsumido,
                AntiguedadAnios = Math.Max(0, anios),
                AntiguedadMeses = Math.Max(0, meses),
                VidaUtilAsignada = dep.VidaUtilAsignada,
                FechaEstimadaFinDepreciacion = activo.FechaAdquisicion.AddYears(dep.VidaUtilAsignada)
            };
        }
    }
}
