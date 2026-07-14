using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Mantenimiento;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class ProgramasMantenimientoController : ControllerBase
	{
		private readonly IProgramaMantenimientoService _programaService;

		public ProgramasMantenimientoController(IProgramaMantenimientoService programaService)
		{
			_programaService = programaService;
		}

		// GET api/programasmantenimiento
		[HttpGet]
		public async Task<IActionResult> ObtenerTodos()
		{
			var resultado = await _programaService.ObtenerTodosAsync();
			return Ok(resultado);
		}

		// POST api/programasmantenimiento — RF-09: definir programa preventivo
		[HttpPost]
		[Authorize(Policy = "ModificarMantenimientosPolicy")]
		public async Task<IActionResult> Crear([FromBody] ProgramaMantenimientoCreateDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
				var resultado = await _programaService.CrearAsync(dto);
				return Ok(resultado);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
		}

		// GET api/programasmantenimiento/proximos-vencer?dias=7 — RF-10
		[HttpGet("proximos-vencer")]
		public async Task<IActionResult> ObtenerProximosAVencer([FromQuery] int dias = 7)
		{
			var resultado = await _programaService.ObtenerProximosAVencerAsync(dias);
			return Ok(resultado);
		}

		// POST api/programasmantenimiento/generar-ordenes — RF-09 (ejecutable manualmente o por un job)
		[HttpPost("generar-ordenes")]
		[Authorize(Policy = "ModificarMantenimientosPolicy")]
		public async Task<IActionResult> GenerarOrdenesPendientes()
		{
			var cantidad = await _programaService.GenerarOrdenesPendientesAsync();
			return Ok(new { ProgramasActualizados = cantidad });
		}

		// POST api/programasmantenimiento/generar-notificaciones?dias=7 — RF-10
		[HttpPost("generar-notificaciones")]
		[Authorize(Policy = "ModificarMantenimientosPolicy")]
		public async Task<IActionResult> GenerarNotificaciones([FromQuery] int dias = 7)
		{
			var cantidad = await _programaService.GenerarNotificacionesVencimientoAsync(dias);
			return Ok(new { NotificacionesGeneradas = cantidad });
		}
	}
}