using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Application.DTOs.Depreciacion;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaResponseDto>> ObtenerTodasAsync();
        Task<CategoriaResponseDto> CrearAsync(CategoriaCreateDto dto);
        Task<CategoriaResponseDto> ActualizarVidaUtilAsync(Guid id, int nuevaVidaUtil);
        Task<bool> EliminarAsync(Guid id);
    }
}
