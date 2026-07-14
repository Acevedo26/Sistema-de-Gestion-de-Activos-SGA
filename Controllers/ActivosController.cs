using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Activos;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ActivosController : ControllerBase
    {
        private readonly IActivoService _activoService;

        public ActivosController(IActivoService activoService)
        {
            _activoService = activoService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var activos = await _activoService.ObtenerTodosAsync();
            return Ok(activos);
        }

        [HttpPost]
        [Authorize(Policy = "ModificarActivosPolicy")]
        public async Task<IActionResult> Crear([FromBody] ActivoRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var activo = await _activoService.CrearActivoAsync(dto);
                return Ok(activo);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "ModificarActivosPolicy")]
        public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActivoUpdateDto dto)
        {
            var resultado = await _activoService.ActualizarActivoAsync(id, dto);

            if (resultado == null)
                return NotFound(new { message = "Activo no encontrado." });

            return Ok(resultado);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "ModificarActivosPolicy")]
        public async Task<IActionResult> Desactivar(Guid id)
        {
            var resultado = await _activoService.DesactivarActivoAsync(id);

            if (!resultado)
                return NotFound(new { message = "Activo no encontrado." });

            return NoContent();
        }
    }
}