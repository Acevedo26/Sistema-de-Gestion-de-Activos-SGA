using Microsoft.EntityFrameworkCore;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Movimientos;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Data;
using Sistema_de_Gestion_de_Activos.Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class MovimientoService : IMovimientoService
    {
        private readonly SgaDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MovimientoService(SgaDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<MovimientoResponseDto> RegistrarMovimientoAsync(MovimientoRequestDto dto)
        {
            var activo = await _context.Activos.FindAsync(dto.ActivoId) ?? throw new KeyNotFoundException("Activo no encontrado.");
            if (activo.Estado is Domain.Enums.EstadoActivo.Baja or Domain.Enums.EstadoActivo.Descartado) throw new InvalidOperationException("No se puede mover un activo dado de baja o descartado.");
            if (activo.UbicacionId != dto.UbicacionOrigenId) throw new InvalidOperationException("La ubicación de origen no coincide con la ubicación actual del activo.");
            if (dto.UbicacionOrigenId == dto.UbicacionDestinoId) throw new InvalidOperationException("La ubicación de destino debe ser diferente.");
            if (!await _context.Ubicaciones.AnyAsync(u => u.Id == dto.UbicacionDestinoId)) throw new KeyNotFoundException("Ubicación destino no encontrada.");
            var movimiento = new Movimiento
            {
                Id = Guid.NewGuid(),
                ActivoId = dto.ActivoId,
                UbicacionOrigenId = dto.UbicacionOrigenId,
                UbicacionDestinoId = dto.UbicacionDestinoId,
                FechaMovimiento = DateTime.UtcNow,
                Observaciones = dto.Observaciones
            };
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userId, out var parsedUserId)) movimiento.UsuarioId = parsedUserId;

            _context.Movimientos.Add(movimiento);
            activo.UbicacionId = dto.UbicacionDestinoId;

            await _context.SaveChangesAsync();

            return new MovimientoResponseDto
            {
                Id = movimiento.Id,
                ActivoId = movimiento.ActivoId,
                UbicacionOrigenId = movimiento.UbicacionOrigenId,
                UbicacionDestinoId = movimiento.UbicacionDestinoId,
                UsuarioId = movimiento.UsuarioId,
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
                    UsuarioId = m.UsuarioId,
                    FechaMovimiento = m.FechaMovimiento,
                    Observaciones = m.Observaciones
                })
                .ToListAsync();
        }
    }
}
