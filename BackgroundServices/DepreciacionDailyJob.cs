using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;

namespace Sistema_de_Gestion_de_Activos.BackgroundServices
{
    public class DepreciacionDailyJob : BackgroundService
    {
        private readonly ILogger<DepreciacionDailyJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        public DepreciacionDailyJob(ILogger<DepreciacionDailyJob> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DepreciacionDailyJob is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var nextRun = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(1);
                var delay = nextRun - now;

                _logger.LogInformation("Next DepreciacionDailyJob run in {DelayTime}", delay);

                await Task.Delay(delay, stoppingToken);

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var depreciacionService = scope.ServiceProvider.GetRequiredService<IDepreciacionService>();
                    var notificacionService = scope.ServiceProvider.GetRequiredService<INotificacionDepreciacionService>();

                    _logger.LogInformation("Running daily recalculation of depreciations.");
                    await depreciacionService.RecalcularTodosAsync();

                    _logger.LogInformation("Generating depreciation alerts.");
                    await notificacionService.GenerarAlertasDepreciacionAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing DepreciacionDailyJob.");
                }
            }
        }
    }
}
