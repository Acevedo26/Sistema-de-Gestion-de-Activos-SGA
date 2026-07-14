using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Domain.Entities;

namespace Sistema_de_Gestion_de_Activos.Repositories.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<bool> ExisteCorreoAsync(string correo);
        Task<Usuario?> ObtenerPorCorreoAsync(string correo);
    }
}
