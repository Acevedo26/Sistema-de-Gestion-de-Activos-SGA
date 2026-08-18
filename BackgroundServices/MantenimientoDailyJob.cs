using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;

namespace Sistema_de_Gestion_de_Activos.BackgroundServices;

public sealed class MantenimientoDailyJob : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<MantenimientoDailyJob> _logger;
    public MantenimientoDailyJob(IServiceProvider services, ILogger<MantenimientoDailyJob> logger) => (_services, _logger) = (services, logger);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var programas = scope.ServiceProvider.GetRequiredService<IProgramaMantenimientoService>();
                await programas.GenerarOrdenesPendientesAsync();
                foreach (var dias in new[] { 7, 14, 30 }) await programas.GenerarNotificacionesVencimientoAsync(dias);
            }
            catch (Exception ex) { _logger.LogError(ex, "Error generando órdenes y notificaciones de mantenimiento."); }

            var now = DateTime.UtcNow;
            var next = now.Date.AddDays(1);
            await Task.Delay(next - now, stoppingToken);
        }
    }
}
