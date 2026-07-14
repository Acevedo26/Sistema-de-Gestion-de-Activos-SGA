using Sistema_de_Gestion_de_Activos.Application.DTOs.Movimientos;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
    public interface IMovimientoService
    {
        Task<MovimientoResponseDto> RegistrarMovimientoAsync(MovimientoRequestDto dto);

        Task<IEnumerable<MovimientoResponseDto>> ObtenerTodosAsync();
    }
}