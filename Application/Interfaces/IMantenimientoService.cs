using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Mantenimiento;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
	public interface IMantenimientoService
	{
		// RF-11: historial completo de mantenimientos
		Task<IEnumerable<MantenimientoResponseDto>> ObtenerTodosAsync();

		// RF-11: historial de mantenimientos de un activo específico
		Task<IEnumerable<MantenimientoResponseDto>> ObtenerPorActivoAsync(Guid activoId);

		Task<MantenimientoResponseDto> ObtenerPorIdAsync(Guid id);

		// RF-08: registro de mantenimiento (preventivo o correctivo)
		Task<MantenimientoResponseDto> RegistrarAsync(MantenimientoCreateDto dto);

		// Marca un mantenimiento en curso como finalizado
		Task<MantenimientoResponseDto> FinalizarAsync(Guid id, MantenimientoFinalizarDto dto);
	}
}