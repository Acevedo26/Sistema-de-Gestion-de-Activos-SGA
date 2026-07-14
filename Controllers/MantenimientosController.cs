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
	public class MantenimientosController : ControllerBase
	{
		private readonly IMantenimientoService _mantenimientoService;

		public MantenimientosController(IMantenimientoService mantenimientoService)
		{
			_mantenimientoService = mantenimientoService;
		}

		// GET api/mantenimientos — RF-11: historial completo
		[HttpGet]
		public async Task<IActionResult> ObtenerTodos()
		{
			var resultado = await _mantenimientoService.ObtenerTodosAsync();
			return Ok(resultado);
		}

		// GET api/mantenimientos/activo/{activoId} — RF-11: historial por activo
		[HttpGet("activo/{activoId}")]
		public async Task<IActionResult> ObtenerPorActivo(Guid activoId)
		{
			var resultado = await _mantenimientoService.ObtenerPorActivoAsync(activoId);
			return Ok(resultado);
		}

		// GET api/mantenimientos/{id}
		[HttpGet("{id}")]
		public async Task<IActionResult> ObtenerPorId(Guid id)
		{
			try
			{
				var resultado = await _mantenimientoService.ObtenerPorIdAsync(id);
				return Ok(resultado);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
		}

		// POST api/mantenimientos — RF-08: registro (Gestor y Técnico pueden registrar)
		[HttpPost]
		[Authorize(Policy = "ModificarMantenimientosPolicy")]
		public async Task<IActionResult> Registrar([FromBody] MantenimientoCreateDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
				var resultado = await _mantenimientoService.RegistrarAsync(dto);
				return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
		}

		// PUT api/mantenimientos/{id}/finalizar
		[HttpPut("{id}/finalizar")]
		[Authorize(Policy = "ModificarMantenimientosPolicy")]
		public async Task<IActionResult> Finalizar(Guid id, [FromBody] MantenimientoFinalizarDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
				var resultado = await _mantenimientoService.FinalizarAsync(id, dto);
				return Ok(resultado);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
		}
	}
}