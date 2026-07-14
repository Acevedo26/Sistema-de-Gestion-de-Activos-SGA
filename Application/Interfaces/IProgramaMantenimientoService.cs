using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Mantenimiento;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
	public interface IProgramaMantenimientoService
	{
		Task<IEnumerable<ProgramaMantenimientoResponseDto>> ObtenerTodosAsync();

		// RF-09: definir un programa de mantenimiento preventivo
		Task<ProgramaMantenimientoResponseDto> CrearAsync(ProgramaMantenimientoCreateDto dto);

		// RF-09: revisa todos los programas activos y genera automáticamente
		// una orden de mantenimiento (Mantenimiento pendiente) cuando se cumple la fecha
		Task<int> GenerarOrdenesPendientesAsync();

		// RF-10: programas cuya próxima fecha está a "diasAnticipacion" días o menos de vencer
		Task<IEnumerable<ProgramaMantenimientoResponseDto>> ObtenerProximosAVencerAsync(int diasAnticipacion);

		// RF-10: genera notificaciones reales (tabla compartida "Notificaciones")
		// para programas próximos a vencer, evitando duplicar una ya existente sin leer
		Task<int> GenerarNotificacionesVencimientoAsync(int diasAnticipacion);
	}
}