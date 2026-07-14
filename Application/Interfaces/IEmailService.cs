using System.Threading.Tasks;

namespace Sistema_de_Gestion_de_Activos.Application.Interfaces
{
    public interface IEmailService
    {
        Task EnviarCorreoRecuperacionAsync(string destinatario, string enlaceRecuperacion);
    }
}
