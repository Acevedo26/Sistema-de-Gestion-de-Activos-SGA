using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Domain.Enums;
using System.Threading.Tasks;

namespace Sistema_de_Gestion_de_Activos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = nameof(NombreRol.Administrador))] // RF-18: Solo lectura para Administrador
    public class AuditoriaController : ControllerBase
    {
        private readonly IAuditoriaService _auditoriaService;

        public AuditoriaController(IAuditoriaService auditoriaService)
        {
            _auditoriaService = auditoriaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var registros = await _auditoriaService.ObtenerTodasAsync();
            return Ok(registros);
        }
    }
}
