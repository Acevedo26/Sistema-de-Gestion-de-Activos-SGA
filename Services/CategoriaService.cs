using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Depreciacion;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Sistema_de_Gestion_de_Activos.Data;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly SgaDbContext _context;

        public CategoriaService(SgaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoriaResponseDto>> ObtenerTodasAsync()
        {
            return await _context.Categorias
                .Select(c => new CategoriaResponseDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    VidaUtil = c.VidaUtil,
                    ValorResidual = c.ValorResidual
                })
                .ToListAsync();
        }

        public async Task<CategoriaResponseDto> ActualizarVidaUtilAsync(Guid id, int nuevaVidaUtil)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) throw new KeyNotFoundException("Categoría no encontrada.");

            categoria.VidaUtil = nuevaVidaUtil;
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();

            return new CategoriaResponseDto
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                VidaUtil = categoria.VidaUtil,
                ValorResidual = categoria.ValorResidual
            };
        }
    }
}
