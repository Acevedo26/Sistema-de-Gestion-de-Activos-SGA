using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Domain.Entities;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Maestros;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
    public interface IUbicacionService
    {
        Task<List<UbicacionDto>> ObtenerTodasAsync();
        Task<Ubicacion?> ObtenerPorIdAsync(Guid id);
        Task<Ubicacion> CrearAsync(Ubicacion ubicacion);
        Task<Ubicacion> ActualizarAsync(Guid id, Ubicacion ubicacion);
        Task<bool> EliminarAsync(Guid id);
    }
}