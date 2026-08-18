using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;

namespace Sistema_de_Gestion_de_Activos.Services;

public sealed class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task EnviarCorreoRecuperacionAsync(string destinatario, string enlaceRecuperacion) =>
        EnviarAsync(destinatario, "Recuperación de contraseña", $"Use el siguiente enlace de un solo uso para restablecer su contraseña: <a href=\"{enlaceRecuperacion}\">Restablecer contraseña</a>.");

    public async Task EnviarAsync(string destinatario, string asunto, string html)
    {
        var host = _configuration["Smtp:Host"];
        var from = _configuration["Smtp:From"];
        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
        {
            _logger.LogWarning("SMTP no configurado; el correo para {Destinatario} no fue enviado.", destinatario);
            return;
        }

        using var client = new SmtpClient(host, _configuration.GetValue("Smtp:Port", 587))
        {
            EnableSsl = _configuration.GetValue("Smtp:EnableSsl", true),
            Credentials = new NetworkCredential(_configuration["Smtp:UserName"], _configuration["Smtp:Password"])
        };
        using var message = new MailMessage(from, destinatario, asunto, html) { IsBodyHtml = true };
        await client.SendMailAsync(message);
    }
}
