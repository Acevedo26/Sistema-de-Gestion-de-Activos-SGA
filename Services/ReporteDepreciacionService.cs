using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Reportes;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Data;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class ReporteDepreciacionService : IReporteDepreciacionService
    {
        private readonly SgaDbContext _context;

        public ReporteDepreciacionService(SgaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReporteDepreciacionDto>> GenerarReporteGeneralAsync(Guid? categoriaId, string? estadoDepreciacion)
        {
            var query = _context.Activos
                .Include(a => a.Categoria)
                .Include(a => a.Depreciacion)
                .Where(a => a.Estado != Domain.Enums.EstadoActivo.Baja && a.Estado != Domain.Enums.EstadoActivo.Descartado)
                .AsQueryable();

            if (categoriaId.HasValue)
            {
                query = query.Where(a => a.CategoriaId == categoriaId.Value);
            }

            var activos = await query.ToListAsync();
            var reporte = new List<ReporteDepreciacionDto>();

            foreach (var a in activos)
            {
                if (a.Depreciacion == null) continue;

                var dep = a.Depreciacion;
                var porcentaje = dep.PorcentajeConsumido;
                
                if (!string.IsNullOrEmpty(estadoDepreciacion))
                {
                    bool match = estadoDepreciacion switch
                    {
                        "<25%" => porcentaje < 25m,
                        "25-75%" => porcentaje >= 25m && porcentaje <= 75m,
                        ">75%" => porcentaje > 75m && porcentaje < 100m,
                        "100%" => porcentaje >= 100m,
                        _ => true
                    };
                    if (!match) continue;
                }

                var tiempo = (DateTime.UtcNow - a.FechaAdquisicion).TotalDays / 365.25;
                var restantes = Math.Max(0, dep.VidaUtilAsignada - (int)tiempo);

                reporte.Add(new ReporteDepreciacionDto
                {
                    Codigo = a.Codigo,
                    Descripcion = a.Descripcion,
                    Categoria = a.Categoria.Nombre,
                    CostoOriginal = a.CostoInicial,
                    ValorActual = dep.ValorActual,
                    ValorResidual = dep.ValorResidual,
                    DepreciacionAcumulada = Math.Max(0, a.CostoInicial - dep.ValorActual),
                    PorcentajeConsumido = porcentaje,
                    AniosVidaUtilRestantes = restantes,
                    FechaEstimadaFinDepreciacion = a.FechaAdquisicion.AddYears(dep.VidaUtilAsignada)
                });
            }

            return reporte;
        }

        public async Task<IEnumerable<AnalisisValorDepartamentoDto>> GenerarAnalisisPorDepartamentoAsync(Guid? ubicacionId)
        {
            var query = _context.Activos
                .Include(a => a.Ubicacion)
                .Include(a => a.Depreciacion)
                .Where(a => a.Estado != Domain.Enums.EstadoActivo.Baja && a.Estado != Domain.Enums.EstadoActivo.Descartado)
                .AsQueryable();

            if (ubicacionId.HasValue)
            {
                query = query.Where(a => a.UbicacionId == ubicacionId.Value);
            }

            var activos = await query.ToListAsync();
            var agrupado = activos
                .GroupBy(a => new { a.Ubicacion.Departamento, a.Ubicacion.Oficina })
                .Select(g => new AnalisisValorDepartamentoDto
                {
                    Departamento = g.Key.Departamento,
                    Oficina = g.Key.Oficina ?? "N/A",
                    ValorOriginalTotal = g.Sum(x => x.CostoInicial),
                    ValorActualTotal = g.Sum(x => x.Depreciacion?.ValorActual ?? x.CostoInicial),
                    DepreciacionTotalAcumulada = g.Sum(x => Math.Max(0, x.CostoInicial - (x.Depreciacion?.ValorActual ?? x.CostoInicial))),
                    PromedioAntiguedadAnios = g.Any() ? (decimal)g.Average(x => (DateTime.UtcNow - x.FechaAdquisicion).TotalDays / 365.25) : 0m
                })
                .ToList();

            return agrupado;
        }

        public async Task<byte[]> ExportarCsvAsync(DateTime desde, DateTime hasta)
        {
            var historial = await _context.HistorialDepreciaciones
                .Include(h => h.Depreciacion)
                .ThenInclude(d => d.Activo)
                .ThenInclude(a => a.Categoria)
                .Where(h => h.FechaConsulta >= desde && h.FechaConsulta <= hasta)
                .OrderBy(h => h.FechaConsulta)
                .ToListAsync();

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, new System.Text.UTF8Encoding(true)); // UTF8 con BOM
            using var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

            var registros = historial.Select(h => new
            {
                CodigoActivo = h.Depreciacion.Activo.Codigo,
                Descripcion = h.Depreciacion.Activo.Descripcion,
                Categoria = h.Depreciacion.Activo.Categoria.Nombre,
                CostoOriginal = h.Depreciacion.Activo.CostoInicial,
                ValorActual = h.ValorActual,
                DepreciacionAcumulada = h.DepreciacionAcumulada,
                FechaConsulta = h.FechaConsulta.ToString("yyyy-MM-dd")
            });

            await csv.WriteRecordsAsync(registros);
            await streamWriter.FlushAsync();
            
            return memoryStream.ToArray();
        }
    }
}
