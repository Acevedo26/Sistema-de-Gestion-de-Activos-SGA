using System.Diagnostics;
using System.Threading.Tasks;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Sistema_de_Gestion_de_Activos.Services
{
    public class EmailServiceMock : IEmailService
    {
        private readonly ILogger<EmailServiceMock> _logger;

        public EmailServiceMock(ILogger<EmailServiceMock> logger)
        {
            _logger = logger;
        }

        public Task EnviarCorreoRecuperacionAsync(string destinatario, string enlaceRecuperacion)
        {
            string mensaje = $"\n--- MOCK EMAIL SENDER ---\nPara: {destinatario}\nAsunto: Recuperación de contraseña\nEnlace: {enlaceRecuperacion}\n-------------------------\n";
            _logger.LogInformation(mensaje);
            Debug.WriteLine(mensaje);
            
            return Task.CompletedTask;
        }
    }
}
