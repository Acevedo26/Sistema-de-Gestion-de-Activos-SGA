using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Data;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Depreciacion;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class AnalisisIntegradoService : IAnalisisIntegradoService
    {
        private readonly SgaDbContext _context;

        public AnalisisIntegradoService(SgaDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> ObtenerRelacionCostoValorAsync(Guid activoId)
        {
            return (await ObtenerAnalisisAsync(activoId)).RelacionCostoValor;
        }

        public async Task<AnalisisMantenimientoDto> ObtenerAnalisisAsync(Guid activoId)
        {
            var activo = await _context.Activos.Include(a => a.Depreciacion).FirstOrDefaultAsync(a => a.Id == activoId);
            if (activo == null) return new AnalisisMantenimientoDto { UmbralAlerta = 30m };
            var costo = await _context.Mantenimientos.Where(m => m.ActivoId == activoId).SumAsync(m => (decimal?)m.Costo) ?? 0m;
            var valor = activo.Depreciacion?.ValorActual ?? activo.CostoInicial;
            var umbralTexto = await _context.ParametrosSistema.Where(p => p.Clave == "UmbralCostoMantenimientoExcesivo").Select(p => p.Valor).FirstOrDefaultAsync();
            var umbral = decimal.TryParse(umbralTexto, out var configurado) ? configurado : 30m;
            return new AnalisisMantenimientoDto
            {
                CostoMantenimientoAcumulado = costo,
                RelacionCostoValor = valor <= 0 ? 0m : Math.Round(costo * 100m / valor, 2),
                UmbralAlerta = umbral
            };
        }
    }
}
