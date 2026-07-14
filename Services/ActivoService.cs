using Microsoft.EntityFrameworkCore;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Activos;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Data;
using Sistema_de_Gestion_de_Activos.Domain.Entities;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class ActivoService : IActivoService
    {
        private readonly SgaDbContext _context;

        public ActivoService(SgaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActivoResponseDto>> ObtenerTodosAsync()
        {
            return await _context.Activos
                .Select(a => new ActivoResponseDto
                {
                    Id = a.Id,
                    Codigo = a.Codigo,
                    Descripcion = a.Descripcion,
                    CategoriaId = a.CategoriaId,
                    UbicacionId = a.UbicacionId,
                    FechaAdquisicion = a.FechaAdquisicion,
                    CostoInicial = a.CostoInicial,
                    Proveedor = a.Proveedor,
                    NumeroSerie = a.NumeroSerie,
                    Estado = a.Estado
                })
                .ToListAsync();
        }

        public async Task<ActivoResponseDto> CrearActivoAsync(ActivoRequestDto dto)
        {
            if (await _context.Activos.AnyAsync(a => a.Codigo == dto.Codigo))
            {
                throw new InvalidOperationException("Ya existe un activo con ese código.");
            }

            var activo = new Activo
            {
                Id = Guid.NewGuid(),
                Codigo = dto.Codigo,
                Descripcion = dto.Descripcion,
                CategoriaId = dto.CategoriaId,
                UbicacionId = dto.UbicacionId,
                FechaAdquisicion = dto.FechaAdquisicion,
                CostoInicial = dto.CostoInicial,
                Proveedor = dto.Proveedor,
                NumeroSerie = dto.NumeroSerie,
                Estado = EstadoActivo.Activo
            };

            _context.Activos.Add(activo);
            await _context.SaveChangesAsync();

            return new ActivoResponseDto
            {
                Id = activo.Id,
                Codigo = activo.Codigo,
                Descripcion = activo.Descripcion,
                CategoriaId = activo.CategoriaId,
                UbicacionId = activo.UbicacionId,
                FechaAdquisicion = activo.FechaAdquisicion,
                CostoInicial = activo.CostoInicial,
                Proveedor = activo.Proveedor,
                NumeroSerie = activo.NumeroSerie,
                Estado = activo.Estado
            };
        }

        public async Task<ActivoResponseDto?> ActualizarActivoAsync(Guid id, ActivoUpdateDto dto)
        {
            var activo = await _context.Activos.FindAsync(id);

            if (activo == null)
                return null;

            if (!string.IsNullOrWhiteSpace(dto.Descripcion))
                activo.Descripcion = dto.Descripcion;

            if (dto.CategoriaId.HasValue)
                activo.CategoriaId = dto.CategoriaId.Value;

            if (dto.UbicacionId.HasValue)
                activo.UbicacionId = dto.UbicacionId.Value;

            if (dto.CostoInicial.HasValue)
                activo.CostoInicial = dto.CostoInicial.Value;

            if (!string.IsNullOrWhiteSpace(dto.Proveedor))
                activo.Proveedor = dto.Proveedor;

            if (!string.IsNullOrWhiteSpace(dto.NumeroSerie))
                activo.NumeroSerie = dto.NumeroSerie;

            if (dto.Estado.HasValue)
                activo.Estado = dto.Estado.Value;

            await _context.SaveChangesAsync();

            return new ActivoResponseDto
            {
                Id = activo.Id,
                Codigo = activo.Codigo,
                Descripcion = activo.Descripcion,
                CategoriaId = activo.CategoriaId,
                UbicacionId = activo.UbicacionId,
                FechaAdquisicion = activo.FechaAdquisicion,
                CostoInicial = activo.CostoInicial,
                Proveedor = activo.Proveedor,
                NumeroSerie = activo.NumeroSerie,
                Estado = activo.Estado
            };
        }

        public async Task<bool> DesactivarActivoAsync(Guid id)
        {
            var activo = await _context.Activos.FindAsync(id);

            if (activo == null)
                return false;

            activo.Estado = EstadoActivo.Baja;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}