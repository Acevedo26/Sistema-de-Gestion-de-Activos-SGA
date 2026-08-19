using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Dashboard;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Data;
using Sistema_de_Gestion_de_Activos.Domain.Entities;
using Sistema_de_Gestion_de_Activos.Domain.Enums;
using Sistema_de_Gestion_de_Activos.Repositories.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IRepository<Activo> _activoRepo;
        private readonly IRepository<Mantenimiento> _mantRepo;
        private readonly IRepository<Auditoria> _auditRepo;
        private readonly SgaDbContext _context; // Para consultas complejas si IRepository no es suficiente

        public DashboardService(
            IRepository<Activo> activoRepo,
            IRepository<Mantenimiento> mantRepo,
            IRepository<Auditoria> auditRepo,
            SgaDbContext context)
        {
            _activoRepo = activoRepo;
            _mantRepo = mantRepo;
            _auditRepo = auditRepo;
            _context = context;
        }

        public async Task<DashboardKpiDto> GetKPIsAsync()
        {
            var kpi = new DashboardKpiDto();
            
            // Todos los activos activos (excluyendo bajas)
            var activos = await _context.Activos
                .Where(a => a.Estado != EstadoActivo.Baja)
                .Include(a => a.Categoria)
                .Include(a => a.Depreciacion)
                .ToListAsync();
            var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            kpi.TotalActivos = activos.Count;
            kpi.ActivosNuevosMes = activos.Count(a => a.FechaAdquisicion >= inicioMes);
            kpi.ValorActualTotal = activos.Sum(a => (a.Depreciacion?.ValorActual ?? a.CostoInicial));
            kpi.DepreciacionAcumuladaTotal = activos.Sum(a => a.CostoInicial - (a.Depreciacion?.ValorActual ?? a.CostoInicial));

            for (int i = 5; i >= 0; i--)
            {
                var mes = DateTime.Now.AddMonths(-i);
                var finDeMes = new DateTime(mes.Year, mes.Month, DateTime.DaysInMonth(mes.Year, mes.Month));
                var activosHastaMes = activos.Where(a => a.FechaAdquisicion <= finDeMes).ToList();
                kpi.TendenciaActivos.Add(activosHastaMes.Count);
                kpi.TendenciaValor.Add(activosHastaMes.Sum(a => a.CostoInicial));
            }

            kpi.ActivosDepreciados = activos.Count(a =>
            {
                if (a.Depreciacion != null && a.Depreciacion.PorcentajeConsumido >= 100) return true;
                if ((a.Depreciacion?.ValorActual ?? a.CostoInicial) <= 0) return true;
                if (a.Categoria != null)
                {
                    var mesesUso = (DateTime.Now - a.FechaAdquisicion).TotalDays / 30.44;
                    if (mesesUso >= a.Categoria.VidaUtil * 12) return true;
                }
                return false;
            });

            var mantenimientos = await _context.Mantenimientos.Include(m => m.Activo).ToListAsync();
            var hoy = DateTime.Now.Date;
            var en7Dias = hoy.AddDays(7);

            kpi.MantenimientosProximos = mantenimientos.Count(m =>
                m.Estado != EstadoMantenimiento.Finalizado &&
                (m.Activo == null || m.Activo.Estado != EstadoActivo.Baja) &&
                m.FechaInicio.Date >= hoy && m.FechaInicio.Date <= en7Dias);

            return kpi;
        }

        public async Task<List<AlertaMantenimientoDto>> GetAlertasMantenimientoAsync()
        {
            var mantenimientos = await _context.Mantenimientos
                .Include(m => m.Activo)
                .Where(m => m.Estado != EstadoMantenimiento.Finalizado && (m.Activo == null || m.Activo.Estado != EstadoActivo.Baja))
                .OrderBy(m => m.FechaInicio)
                .Take(5)
                .ToListAsync();

            return mantenimientos.Select(m => new AlertaMantenimientoDto
            {
                ActivoId = m.ActivoId,
                NombreActivo = m.Activo?.Descripcion ?? "Activo Desconocido",
                FechaVencimiento = m.FechaInicio
            }).ToList();
        }

        public async Task<List<ActividadRecienteDto>> GetActividadRecienteAsync()
        {
            var registros = await _context.Set<Auditoria>()
                .Include(a => a.Usuario)
                .OrderByDescending(a => a.FechaAccion)
                .Take(5)
                .ToListAsync();

            var actividades = new List<ActividadRecienteDto>();

            foreach (var r in registros)
            {
                var tipoEvento = r.Accion.ToString();
                var desc = "";

                if (r.TablaAfectada.Contains("Activo", StringComparison.OrdinalIgnoreCase))
                {
                    if (r.Accion == AccionAuditoria.INSERT) tipoEvento = "Activo Creado";
                    else if (r.Accion == AccionAuditoria.UPDATE) tipoEvento = "Activo Modificado";
                    else if (r.Accion == AccionAuditoria.DELETE) tipoEvento = "Baja de Activo";
                    desc = "Operación en Activos"; // Sería ideal extraer el nombre del JSON, pero lo mantenemos simple.
                }
                else if (r.TablaAfectada.Contains("Mantenimiento", StringComparison.OrdinalIgnoreCase))
                {
                    if (r.Accion == AccionAuditoria.UPDATE) tipoEvento = "Mantenimiento Actualizado";
                    if (r.Accion == AccionAuditoria.INSERT) tipoEvento = "Mantenimiento Programado";
                    desc = "Operación en Mantenimientos";
                }
                else
                {
                    tipoEvento = $"{r.Accion} en {r.TablaAfectada}";
                }

                var tiempo = GetTiempoRelativo(r.FechaAccion);
                var usuario = string.IsNullOrWhiteSpace(r.Usuario?.Nombre) ? "Sistema" : r.Usuario.Nombre;

                actividades.Add(new ActividadRecienteDto
                {
                    Id = r.Id,
                    TipoEvento = tipoEvento,
                    Descripcion = desc,
                    TiempoRelativo = $"{tiempo} por {usuario}",
                    FechaOriginal = r.FechaAccion
                });
            }

            return actividades;
        }

        private string GetTiempoRelativo(DateTime fecha)
        {
            var ts = DateTime.UtcNow - fecha;
            if (ts.TotalMinutes < 60) return $"Hace {(int)ts.TotalMinutes} min";
            if (ts.TotalHours < 24) return $"Hace {(int)ts.TotalHours}h";
            if (ts.TotalDays < 2) return "Ayer";
            return $"Hace {(int)ts.TotalDays} días";
        }
        
        public static string FormatearMoneda(decimal valor)
        {
            return valor.ToString("C2", new CultureInfo("es-CR")).Replace("¢", "₡");
        }
    }
}