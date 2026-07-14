using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Movimientos;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MovimientosController : ControllerBase
    {
        private readonly IMovimientoService _movimientoService;

        public MovimientosController(IMovimientoService movimientoService)
        {
            _movimientoService = movimientoService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var movimientos = await _movimientoService.ObtenerTodosAsync();
            return Ok(movimientos);
        }

        [HttpPost]
        [Authorize(Policy = "ModificarActivosPolicy")]
        public async Task<IActionResult> Registrar([FromBody] MovimientoRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _movimientoService.RegistrarMovimientoAsync(dto);

            return Ok(resultado);
        }
    }
}