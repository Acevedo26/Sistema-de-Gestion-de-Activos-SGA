using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Mantenimiento;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Data;
using Sistema_de_Gestion_de_Activos.Domain.Entities;
using Sistema_de_Gestion_de_Activos.Domain.Enums;

namespace Sistema_de_Gestion_de_Activos.Services
{
	public class MantenimientoService : IMantenimientoService
	{
		private readonly SgaDbContext _context;

		public MantenimientoService(SgaDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<MantenimientoResponseDto>> ObtenerTodosAsync()
		{
			return await _context.Mantenimientos
				.Include(m => m.Activo)
				.Include(m => m.Tecnico)
				.OrderByDescending(m => m.FechaInicio)
				.Select(m => MapearADto(m))
				.ToListAsync();
		}

		public async Task<IEnumerable<MantenimientoResponseDto>> ObtenerPorActivoAsync(Guid activoId)
		{
			return await _context.Mantenimientos
				.Include(m => m.Activo)
				.Include(m => m.Tecnico)
				.Where(m => m.ActivoId == activoId)
				.OrderByDescending(m => m.FechaInicio)
				.Select(m => MapearADto(m))
				.ToListAsync();
		}

		public async Task<MantenimientoResponseDto> ObtenerPorIdAsync(Guid id)
		{
			var mantenimiento = await _context.Mantenimientos
				.Include(m => m.Activo)
				.Include(m => m.Tecnico)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (mantenimiento == null)
				throw new KeyNotFoundException("Mantenimiento no encontrado.");

			return MapearADto(mantenimiento);
		}

		public async Task<MantenimientoResponseDto> RegistrarAsync(MantenimientoCreateDto dto)
		{
			var activo = await _context.Activos.FindAsync(dto.ActivoId);
			if (activo == null)
				throw new KeyNotFoundException("Activo no encontrado.");

			var tecnico = await _context.Usuarios.FindAsync(dto.TecnicoId);
			if (tecnico == null)
				throw new KeyNotFoundException("Técnico no encontrado.");

			var mantenimiento = new Mantenimiento
			{
				Id = Guid.NewGuid(),
				ActivoId = dto.ActivoId,
				TecnicoId = dto.TecnicoId,
				Tipo = dto.Tipo,
				FechaInicio = dto.FechaInicio,
				FechaFin = dto.FechaFin,
				Costo = dto.Costo,
				Descripcion = dto.Descripcion,
				Observaciones = dto.Observaciones,
				Estado = dto.FechaFin.HasValue ? EstadoMantenimiento.Finalizado : EstadoMantenimiento.EnProgreso
			};

			await _context.Mantenimientos.AddAsync(mantenimiento);

			// Mientras el mantenimiento está en curso, el activo queda marcado como "Mantenimiento" (RF-01/RF-02)
			if (!dto.FechaFin.HasValue && activo.Estado == EstadoActivo.Activo)
			{
				activo.Estado = EstadoActivo.Mantenimiento;
				_context.Activos.Update(activo);
			}

			await _context.SaveChangesAsync();

			mantenimiento.Activo = activo;
			mantenimiento.Tecnico = tecnico;
			return MapearADto(mantenimiento);
		}

		public async Task<MantenimientoResponseDto> FinalizarAsync(Guid id, MantenimientoFinalizarDto dto)
		{
			var mantenimiento = await _context.Mantenimientos
				.Include(m => m.Activo)
				.Include(m => m.Tecnico)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (mantenimiento == null)
				throw new KeyNotFoundException("Mantenimiento no encontrado.");

			mantenimiento.FechaFin = dto.FechaFin;
			mantenimiento.Costo = dto.Costo;
			mantenimiento.Estado = EstadoMantenimiento.Finalizado;

			// Si el activo no tiene otros mantenimientos abiertos, vuelve a estado Activo
			var tieneOtrosAbiertos = await _context.Mantenimientos
				.AnyAsync(m => m.ActivoId == mantenimiento.ActivoId
							&& m.Id != mantenimiento.Id
							&& m.Estado != EstadoMantenimiento.Finalizado);

			if (!tieneOtrosAbiertos && mantenimiento.Activo.Estado == EstadoActivo.Mantenimiento)
			{
				mantenimiento.Activo.Estado = EstadoActivo.Activo;
				_context.Activos.Update(mantenimiento.Activo);
			}

			_context.Mantenimientos.Update(mantenimiento);
			await _context.SaveChangesAsync();

			return MapearADto(mantenimiento);
		}

		private static MantenimientoResponseDto MapearADto(Mantenimiento m)
		{
			return new MantenimientoResponseDto
			{
				Id = m.Id,
				ActivoId = m.ActivoId,
				ActivoDescripcion = m.Activo?.Descripcion ?? string.Empty,
				TecnicoId = m.TecnicoId,
				TecnicoNombre = m.Tecnico?.Nombre ?? string.Empty,
				Tipo = m.Tipo,
				Estado = m.Estado,
				FechaInicio = m.FechaInicio,
				FechaFin = m.FechaFin,
				Costo = m.Costo,
				Descripcion = m.Descripcion,
				Observaciones = m.Observaciones
			};
		}
	}
}