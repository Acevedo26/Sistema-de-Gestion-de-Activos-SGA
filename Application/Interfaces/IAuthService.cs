using System.Threading.Tasks;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
    public interface IAuthService
    {
        Task SolicitarRecuperacionAsync(string correo);
        Task<bool> RestablecerContrasenaAsync(string token, string nuevaContrasena);
        Task<Sistema_de_Gestion_de_Activos.Domain.Entities.Usuario?> ValidarCredencialesAsync(string correo, string password);
    }
}
