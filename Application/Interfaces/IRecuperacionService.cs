using System.Threading.Tasks;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
    public interface IRecuperacionService
    {
        Task SolicitarRecuperacionAsync(string email);
        Task<bool> ValidarTokenAsync(string token);
        Task RestablecerPasswordAsync(string token, string nuevaPassword);
    }
}
