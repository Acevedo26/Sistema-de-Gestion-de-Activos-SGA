using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Domain.Entities;
using Sistema_de_Gestion_de_Activos.Repositories.Interfaces;
using System.Linq;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class UbicacionService : IUbicacionService
    {
        private readonly IRepository<Ubicacion> _repository;

        public UbicacionService(IRepository<Ubicacion> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Ubicacion>> ObtenerTodasAsync()
        {
            var data = await _repository.GetAllAsync();
            return data.OrderBy(u => u.Edificio).ThenBy(u => u.Departamento);
        }

        public async Task<Ubicacion?> ObtenerPorIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Ubicacion> CrearAsync(Ubicacion ubicacion)
        {
            ubicacion.Id = Guid.NewGuid();
            await _repository.AddAsync(ubicacion);
            await _repository.SaveChangesAsync();
            return ubicacion;
        }

        public async Task<Ubicacion> ActualizarAsync(Guid id, Ubicacion ubicacion)
        {
            var existente = await _repository.GetByIdAsync(id);
            if (existente == null) throw new KeyNotFoundException("Ubicación no encontrada.");

            existente.Edificio = ubicacion.Edificio;
            existente.Departamento = ubicacion.Departamento;
            existente.Oficina = ubicacion.Oficina;

            _repository.Update(existente);
            await _repository.SaveChangesAsync();
            return existente;
        }

        public async Task<bool> EliminarAsync(Guid id)
        {
            var existente = await _repository.GetByIdAsync(id);
            if (existente == null) return false;

            _repository.Remove(existente);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}