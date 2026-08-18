using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Depreciacion;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // GET api/categorias — lista todas las categorías con su vida útil
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var categorias = await _categoriaService.ObtenerTodasAsync();
            return Ok(categorias);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Crear([FromBody] CategoriaCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try { return Created($"api/categorias", await _categoriaService.CrearAsync(dto)); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        // PUT api/categorias/{id} — solo Administrador puede cambiar la vida útil (RF-20)
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ActualizarVidaUtil(Guid id, [FromBody] CategoriaVidaUtilUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var resultado = await _categoriaService.ActualizarVidaUtilAsync(id, dto.VidaUtil);
                return Ok(resultado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            try { return await _categoriaService.EliminarAsync(id) ? NoContent() : NotFound(); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}
