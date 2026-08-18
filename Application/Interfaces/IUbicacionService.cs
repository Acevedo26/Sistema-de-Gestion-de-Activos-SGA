using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Domain.Entities;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
    public interface IUbicacionService
    {
        Task<IEnumerable<Ubicacion>> ObtenerTodasAsync();
        Task<Ubicacion?> ObtenerPorIdAsync(Guid id);
        Task<Ubicacion> CrearAsync(Ubicacion ubicacion);
        Task<Ubicacion> ActualizarAsync(Guid id, Ubicacion ubicacion);
        Task<bool> EliminarAsync(Guid id);
    }
}