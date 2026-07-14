using Microsoft.EntityFrameworkCore;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Movimientos;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Data;
using Sistema_de_Gestion_de_Activos.Domain.Entities;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class MovimientoService : IMovimientoService
    {
        private readonly SgaDbContext _context;

        public MovimientoService(SgaDbContext context)
        {
            _context = context;
        }

        public async Task<MovimientoResponseDto> RegistrarMovimientoAsync(MovimientoRequestDto dto)
        {
            var movimiento = new Movimiento
            {
                Id = Guid.NewGuid(),
                ActivoId = dto.ActivoId,
                UbicacionOrigenId = dto.UbicacionOrigenId,
                UbicacionDestinoId = dto.UbicacionDestinoId,
                FechaMovimiento = DateTime.UtcNow,
                Observaciones = dto.Observaciones
            };

            _context.Movimientos.Add(movimiento);

            await _context.SaveChangesAsync();

            return new MovimientoResponseDto
            {
                Id = movimiento.Id,
                ActivoId = movimiento.ActivoId,
                UbicacionOrigenId = movimiento.UbicacionOrigenId,
                UbicacionDestinoId = movimiento.UbicacionDestinoId,
                FechaMovimiento = movimiento.FechaMovimiento,
                Observaciones = movimiento.Observaciones
            };
        }

        public async Task<IEnumerable<MovimientoResponseDto>> ObtenerTodosAsync()
        {
            return await _context.Movimientos
                .Select(m => new MovimientoResponseDto
                {
                    Id = m.Id,
                    ActivoId = m.ActivoId,
                    UbicacionOrigenId = m.UbicacionOrigenId,
                    UbicacionDestinoId = m.UbicacionDestinoId,
                    FechaMovimiento = m.FechaMovimiento,
                    Observaciones = m.Observaciones
                })
                .ToListAsync();
        }
    }
}