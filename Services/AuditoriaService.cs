using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Auditoria;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Domain.Entities;
using Sistema_de_Gestion_de_Activos.Repositories.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly IRepository<Auditoria> _auditoriaRepository;

        public AuditoriaService(IRepository<Auditoria> auditoriaRepository)
        {
            _auditoriaRepository = auditoriaRepository;
        }

        public async Task<IEnumerable<AuditoriaResponseDto>> ObtenerTodasAsync()
        {
            var registros = await _auditoriaRepository.GetAllAsync();
            return registros.OrderByDescending(a => a.FechaAccion).Select(a => new AuditoriaResponseDto
            {
                Id = a.Id,
                UsuarioId = a.UsuarioId,
                TablaAfectada = a.TablaAfectada,
                Accion = a.Accion,
                ValoresAnteriores = a.ValoresAnteriores,
                ValoresNuevos = a.ValoresNuevos,
                FechaAccion = a.FechaAccion
            });
        }
    }
}
