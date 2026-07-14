using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sistema_de_Gestion_de_Activos.Application.Interfaces;

namespace Sistema_de_Gestion_de_Activos.BackgroundServices
{
    public class SnapshotMensualJob : BackgroundService
    {
        private readonly ILogger<SnapshotMensualJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SnapshotMensualJob(ILogger<SnapshotMensualJob> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SnapshotMensualJob is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                
                // Buscar el primer día del siguiente mes
                var nextMonth = now.Month == 12 ? 1 : now.Month + 1;
                var nextYear = now.Month == 12 ? now.Year + 1 : now.Year;
                var nextRun = new DateTime(nextYear, nextMonth, 1, 0, 0, 0, DateTimeKind.Utc);
                
                var delay = nextRun - now;

                _logger.LogInformation("Next SnapshotMensualJob run in {DelayTime}", delay);

                await Task.Delay(delay, stoppingToken);

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var historialService = scope.ServiceProvider.GetRequiredService<IHistorialDepreciacionService>();

                    _logger.LogInformation("Generating monthly depreciation snapshots.");
                    await historialService.GenerarSnapshotMensualAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing SnapshotMensualJob.");
                }
            }
        }
    }
}
