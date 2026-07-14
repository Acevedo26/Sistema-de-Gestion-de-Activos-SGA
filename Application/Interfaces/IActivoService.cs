using Sistema_de_Gestion_de_Activos.Application.DTOs.Activos;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
    public interface IActivoService
    {
        Task<ActivoResponseDto> CrearActivoAsync(ActivoRequestDto dto);

        Task<ActivoResponseDto?> ActualizarActivoAsync(Guid id, ActivoUpdateDto dto);

        Task<bool> DesactivarActivoAsync(Guid id);

        Task<IEnumerable<ActivoResponseDto>> ObtenerTodosAsync();
    }
}