using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Usuarios;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace Sistema_de_Gestion_de_Activos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = nameof(NombreRol.Administrador))] // Solo el Administrador puede gestionar usuarios (RF-16)
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _usuarioService.ObtenerTodosAsync();
            return Ok(usuarios);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioRequestDto dto)
        {
            try
            {
                var response = await _usuarioService.CrearUsuarioAsync(dto);
                return StatusCode(201, response); // 201 Created
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message }); // 400 Bad Request si el correo ya existe
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UsuarioUpdateDto dto)
        {
            var response = await _usuarioService.ActualizarUsuarioAsync(id, dto);
            if (response == null) return NotFound(new { mensaje = "Usuario no encontrado" });

            return Ok(response);
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> Desactivar(Guid id)
        {
            var success = await _usuarioService.DesactivarUsuarioAsync(id);
            if (!success) return NotFound(new { mensaje = "Usuario no encontrado" });

            return NoContent(); // 204 No Content
        }

        [HttpPatch("{id}/estado/activar")]
        public async Task<IActionResult> Activar(Guid id)
        {
            var success = await _usuarioService.ActivarUsuarioAsync(id);
            if (!success) return NotFound(new { mensaje = "Usuario no encontrado" });

            return NoContent();
        }
    }
}
