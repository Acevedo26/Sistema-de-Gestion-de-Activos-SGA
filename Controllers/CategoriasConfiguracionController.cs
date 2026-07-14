using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Controllers
{
    [ApiController]
    [Route("api/categorias")]
    // [Authorize(Roles = "Administrador")] // Descomentar al integrar autenticación
    public class CategoriasConfiguracionController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasConfiguracionController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // RF-20: Configuración de Tablas de Depreciación por Categoría
        [HttpPut("{id}/vida-util")]
        public async Task<IActionResult> ActualizarVidaUtil(Guid id, [FromBody] int nuevaVidaUtil)
        {
            if (nuevaVidaUtil <= 0) return BadRequest("La vida útil debe ser mayor a 0.");

            try
            {
                await _categoriaService.ActualizarVidaUtilAsync(id, nuevaVidaUtil);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
