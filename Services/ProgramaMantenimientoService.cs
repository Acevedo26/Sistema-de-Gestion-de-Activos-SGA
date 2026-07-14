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
	public class ProgramaMantenimientoService : IProgramaMantenimientoService
	{
		private readonly SgaDbContext _context;

		public ProgramaMantenimientoService(SgaDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<ProgramaMantenimientoResponseDto>> ObtenerTodosAsync()
		{
			return await _context.ProgramasMantenimiento
				.Include(p => p.Activo)
				.Include(p => p.TecnicoAsignado)
				.OrderBy(p => p.ProximaFecha)
				.Select(p => MapearADto(p))
				.ToListAsync();
		}

		public async Task<ProgramaMantenimientoResponseDto> CrearAsync(ProgramaMantenimientoCreateDto dto)
		{
			var activo = await _context.Activos.FindAsync(dto.ActivoId);
			if (activo == null)
				throw new KeyNotFoundException("Activo no encontrado.");

			Usuario? tecnicoAsignado = null;
			if (dto.TecnicoAsignadoId.HasValue)
			{
				tecnicoAsignado = await _context.Usuarios.FindAsync(dto.TecnicoAsignadoId.Value);
				if (tecnicoAsignado == null)
					throw new KeyNotFoundException("Técnico asignado no encontrado.");
			}

			var programa = new ProgramaMantenimiento
			{
				Id = Guid.NewGuid(),
				ActivoId = dto.ActivoId,
				Titulo = dto.Titulo,
				FrecuenciaDias = dto.FrecuenciaDias,
				ProximaFecha = dto.ProximaFecha,
				Estado = EstadoPrograma.Activo,
				TecnicoAsignadoId = dto.TecnicoAsignadoId
			};

			await _context.ProgramasMantenimiento.AddAsync(programa);
			await _context.SaveChangesAsync();

			programa.Activo = activo;
			programa.TecnicoAsignado = tecnicoAsignado;
			return MapearADto(programa);
		}

		// RF-09: revisa los programas activos vencidos. Si el programa tiene técnico
		// asignado, crea automáticamente la orden real (Mantenimiento, Estado=Pendiente).
		// Si no tiene técnico asignado, solo avanza el ciclo (no puede crear la orden
		// porque tecnico_id es obligatorio en la tabla de mantenimientos).
		public async Task<int> GenerarOrdenesPendientesAsync()
		{
			var hoy = DateTime.UtcNow;

			var vencidos = await _context.ProgramasMantenimiento
				.Where(p => p.Estado == EstadoPrograma.Activo && p.ProximaFecha <= hoy)
				.ToListAsync();

			int ordenesCreadas = 0;

			foreach (var programa in vencidos)
			{
				if (programa.TecnicoAsignadoId.HasValue)
				{
					var orden = new Mantenimiento
					{
						Id = Guid.NewGuid(),
						ActivoId = programa.ActivoId,
						TecnicoId = programa.TecnicoAsignadoId.Value,
						Tipo = TipoMantenimiento.Preventivo,
						Estado = EstadoMantenimiento.Pendiente,
						FechaInicio = programa.ProximaFecha,
						Costo = 0m,
						Descripcion = $"Orden generada automáticamente por el programa preventivo: {programa.Titulo}"
					};
					await _context.Mantenimientos.AddAsync(orden);
					ordenesCreadas++;
				}

				programa.ProximaFecha = programa.ProximaFecha.AddDays(programa.FrecuenciaDias);
				_context.ProgramasMantenimiento.Update(programa);
			}

			await _context.SaveChangesAsync();
			return ordenesCreadas;
		}

		public async Task<IEnumerable<ProgramaMantenimientoResponseDto>> ObtenerProximosAVencerAsync(int diasAnticipacion)
		{
			var limite = DateTime.UtcNow.AddDays(diasAnticipacion);

			return await _context.ProgramasMantenimiento
				.Include(p => p.Activo)
				.Include(p => p.TecnicoAsignado)
				.Where(p => p.Estado == EstadoPrograma.Activo && p.ProximaFecha <= limite)
				.OrderBy(p => p.ProximaFecha)
				.Select(p => MapearADto(p))
				.ToListAsync();
		}

		// RF-10: genera notificaciones reales (tabla compartida "Notificaciones")
		// para programas próximos a vencer, evitando duplicar una ya existente sin leer.
		public async Task<int> GenerarNotificacionesVencimientoAsync(int diasAnticipacion)
		{
			var proximos = (await ObtenerProximosAVencerAsync(diasAnticipacion)).ToList();
			int generadas = 0;

			foreach (var programa in proximos)
			{
				var yaExiste = await _context.Notificaciones.AnyAsync(n =>
					n.ProgramaMantenimientoId == programa.Id &&
					n.Tipo == TipoNotificacion.VencimientoMantenimiento &&
					!n.Leida);

				if (yaExiste) continue;

				var notificacion = new Notificacion
				{
					Id = Guid.NewGuid(),
					Tipo = TipoNotificacion.VencimientoMantenimiento,
					ActivoId = programa.ActivoId,
					ProgramaMantenimientoId = programa.Id,
					Mensaje = $"El programa '{programa.Titulo}' de {programa.ActivoDescripcion} vence el {programa.ProximaFecha:dd/MM/yyyy}.",
					FechaGeneracion = DateTime.UtcNow,
					Leida = false,
					EnviadaPorCorreo = false
				};

				await _context.Notificaciones.AddAsync(notificacion);
				generadas++;
			}

			await _context.SaveChangesAsync();
			return generadas;
		}

		private static ProgramaMantenimientoResponseDto MapearADto(ProgramaMantenimiento p)
		{
			return new ProgramaMantenimientoResponseDto
			{
				Id = p.Id,
				ActivoId = p.ActivoId,
				ActivoDescripcion = p.Activo?.Descripcion ?? string.Empty,
				Titulo = p.Titulo,
				FrecuenciaDias = p.FrecuenciaDias,
				ProximaFecha = p.ProximaFecha,
				Estado = p.Estado,
				TecnicoAsignadoId = p.TecnicoAsignadoId,
				TecnicoAsignadoNombre = p.TecnicoAsignado?.Nombre
			};
		}
	}
}