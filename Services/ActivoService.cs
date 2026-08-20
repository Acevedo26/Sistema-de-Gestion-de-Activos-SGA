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
        private readonly IDepreciacionService _depreciacionService;

        public ActivoService(SgaDbContext context, IDepreciacionService depreciacionService)
        {
            _context = context;
            _depreciacionService = depreciacionService;
        }

        public async Task<IEnumerable<ActivoResponseDto>> ObtenerTodosAsync()
        {
            return await _context.Activos
                .Include(a => a.Depreciacion)
                .Include(a => a.Responsable)
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
                    Estado = a.Estado,
                    ResponsableId = a.ResponsableId,
                    ResponsableNombre = a.Responsable != null ? a.Responsable.Nombre : null,
                    ValorActual = a.Depreciacion != null ? a.Depreciacion.ValorActual : a.CostoInicial,
                    PorcentajeDepreciacion = a.Depreciacion != null ? a.Depreciacion.PorcentajeConsumido : 0
                })
                .ToListAsync();
        }

        public async Task<ActivoResponseDto?> ObtenerPorIdAsync(Guid id)
        {
            var activo = await _context.Activos
                .Include(a => a.Depreciacion)
                .Include(a => a.Responsable)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activo == null)
                return null;

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
                Estado = activo.Estado,
                ResponsableId = activo.ResponsableId,
                ResponsableNombre = activo.Responsable != null ? activo.Responsable.Nombre : null,
                ValorActual = activo.Depreciacion != null ? activo.Depreciacion.ValorActual : activo.CostoInicial,
                PorcentajeDepreciacion = activo.Depreciacion != null ? activo.Depreciacion.PorcentajeConsumido : 0
            };
        }

        public async Task<ActivoDetalleCompletoDto?> ObtenerDetalleCompletoAsync(Guid id)
        {
            var activo = await _context.Activos
                .Include(a => a.Depreciacion)
                .Include(a => a.Responsable)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activo == null)
                return null;

            var mantenimientos = await _context.Mantenimientos
                .Include(m => m.Tecnico)
                .Where(m => m.ActivoId == id)
                .OrderByDescending(m => m.FechaInicio)
                .ToListAsync();

            var movimientos = await _context.Movimientos
                .Include(m => m.UbicacionOrigen)
                .Include(m => m.UbicacionDestino)
                .Include(m => m.Usuario)
                .Where(m => m.ActivoId == id)
                .OrderByDescending(m => m.FechaMovimiento)
                .ToListAsync();

            var dto = new ActivoDetalleCompletoDto
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
                Estado = activo.Estado,
                ResponsableId = activo.ResponsableId,
                ResponsableNombre = activo.Responsable != null ? activo.Responsable.Nombre : null,
                ValorActual = activo.Depreciacion != null ? activo.Depreciacion.ValorActual : activo.CostoInicial,
                PorcentajeDepreciacion = activo.Depreciacion != null ? activo.Depreciacion.PorcentajeConsumido : 0
            };

            dto.Mantenimientos = mantenimientos.Take(10).Select(m => new MantenimientoResumenDto
            {
                Id = m.Id,
                Tipo = m.Tipo,
                Descripcion = m.Descripcion ?? string.Empty,
                TecnicoNombre = m.Tecnico?.Nombre ?? "Sin Técnico",
                FechaInicio = m.FechaInicio,
                FechaFin = m.FechaFin,
                Costo = m.Costo,
                Estado = m.Estado
            }).ToList();

            dto.Movimientos = movimientos.Take(10).Select(m => new MovimientoResumenDto
            {
                Id = m.Id,
                FechaMovimiento = m.FechaMovimiento,
                UsuarioOperador = m.Usuario?.Nombre ?? "Sistema",
                UbicacionAnterior = $"{m.UbicacionOrigen.Edificio} - {m.UbicacionOrigen.Departamento}",
                UbicacionNueva = $"{m.UbicacionDestino.Edificio} - {m.UbicacionDestino.Departamento}",
                Observaciones = m.Observaciones ?? string.Empty
            }).ToList();

            decimal costoMantenimiento = mantenimientos
                .Where(m => m.Estado == EstadoMantenimiento.Finalizado)
                .Sum(m => m.Costo);

            decimal porcentajeCostoVsValor = 0;
            if (dto.ValorActual > 0)
            {
                porcentajeCostoVsValor = (costoMantenimiento / dto.ValorActual) * 100;
            }

            bool alertaExcesivo = porcentajeCostoVsValor > 30;
            
            string recomendacion = "Mantenimiento rentable";
            if (alertaExcesivo)
            {
                recomendacion = "Reemplazo recomendado";
            }
            else if (porcentajeCostoVsValor > 15)
            {
                recomendacion = "Vigilar costo de mantenimiento";
            }

            dto.Analisis = new AnalisisRendimientoDto
            {
                CostoTotalMantenimiento = costoMantenimiento,
                ValorActual = dto.ValorActual,
                PorcentajeCostoVsValor = porcentajeCostoVsValor,
                AlertaCostoExcesivo = alertaExcesivo,
                Recomendacion = recomendacion
            };

            return dto;
        }

        public async Task<ActivoResponseDto> CrearActivoAsync(ActivoRequestDto dto)
        {
            if (dto.CostoInicial <= 0) throw new InvalidOperationException("El costo inicial debe ser mayor que cero.");
            if (await _context.Activos.AnyAsync(a => a.Codigo == dto.Codigo))
            {
                throw new InvalidOperationException("Ya existe un activo con ese código.");
            }
            if (!await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId)) throw new InvalidOperationException("La categoría no existe.");
            if (!await _context.Ubicaciones.AnyAsync(u => u.Id == dto.UbicacionId)) throw new InvalidOperationException("La ubicación no existe.");
            if (dto.ResponsableId.HasValue && !await _context.Usuarios.AnyAsync(u => u.Id == dto.ResponsableId.Value && u.Estado == EstadoUsuario.Activo)) throw new InvalidOperationException("El responsable no existe o está inactivo.");

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
                ResponsableId = dto.ResponsableId,
                FechaAsignacion = dto.ResponsableId.HasValue ? DateTime.UtcNow : null,
                ObservacionesAsignacion = dto.ObservacionesAsignacion,
                Estado = EstadoActivo.Activo
            };

            _context.Activos.Add(activo);
            await _context.SaveChangesAsync();
            await _depreciacionService.InicializarDepreciacionAsync(activo.Id, activo.CostoInicial, activo.CategoriaId);

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
                Estado = activo.Estado,
                ResponsableId = activo.ResponsableId,
                ValorActual = activo.CostoInicial, // Recién creado
                PorcentajeDepreciacion = 0
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
            {
                if (dto.CostoInicial <= 0) throw new InvalidOperationException("El costo inicial debe ser mayor que cero.");
                activo.CostoInicial = dto.CostoInicial.Value;
            }

            if (dto.FechaAdquisicion.HasValue)
            {
                if (dto.FechaAdquisicion.Value > DateTime.UtcNow) throw new InvalidOperationException("La fecha de adquisición no puede estar en el futuro.");
                activo.FechaAdquisicion = dto.FechaAdquisicion.Value;
            }

            if (!string.IsNullOrWhiteSpace(dto.Proveedor))
                activo.Proveedor = dto.Proveedor;

            if (!string.IsNullOrWhiteSpace(dto.NumeroSerie))
                activo.NumeroSerie = dto.NumeroSerie;

            if (dto.Estado.HasValue)
                activo.Estado = dto.Estado.Value;

            if (dto.ResponsableId.HasValue && activo.ResponsableId != dto.ResponsableId)
            {
                if (!await _context.Usuarios.AnyAsync(u => u.Id == dto.ResponsableId && u.Estado == EstadoUsuario.Activo)) throw new InvalidOperationException("El responsable no existe o está inactivo.");
                activo.ResponsableId = dto.ResponsableId;
                activo.FechaAsignacion = DateTime.UtcNow;
                activo.ObservacionesAsignacion = dto.ObservacionesAsignacion;
            }

            await _context.SaveChangesAsync();
            await _depreciacionService.RecalcularAsync(id);

            var activoActualizado = await _context.Activos
                .Include(a => a.Depreciacion)
                .FirstOrDefaultAsync(a => a.Id == id);

            return new ActivoResponseDto
            {
                Id = activoActualizado!.Id,
                Codigo = activoActualizado.Codigo,
                Descripcion = activoActualizado.Descripcion,
                CategoriaId = activoActualizado.CategoriaId,
                UbicacionId = activoActualizado.UbicacionId,
                FechaAdquisicion = activoActualizado.FechaAdquisicion,
                CostoInicial = activoActualizado.CostoInicial,
                Proveedor = activoActualizado.Proveedor,
                NumeroSerie = activoActualizado.NumeroSerie,
                Estado = activoActualizado.Estado,
                ResponsableId = activoActualizado.ResponsableId,
                ValorActual = activoActualizado.Depreciacion != null ? activoActualizado.Depreciacion.ValorActual : activoActualizado.CostoInicial,
                PorcentajeDepreciacion = activoActualizado.Depreciacion != null ? activoActualizado.Depreciacion.PorcentajeConsumido : 0
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
