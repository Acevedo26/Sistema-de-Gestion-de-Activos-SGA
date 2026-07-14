using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Gestion_de_Activos.Data;
using Sistema_de_Gestion_de_Activos.Domain.Entities;
using Sistema_de_Gestion_de_Activos.Repositories.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Repositories.Implementations
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(SgaDbContext context) : base(context)
        {
        }

        public async Task<bool> ExisteCorreoAsync(string correo)
        {
            return await _dbSet.AnyAsync(u => u.Correo == correo);
        }

        public async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Correo == correo);
        }
    }
}
