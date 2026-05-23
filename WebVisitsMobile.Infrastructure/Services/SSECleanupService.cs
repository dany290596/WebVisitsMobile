using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebVisitsMobile.Infrastructure.Services
{
    public class SSECleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SSECleanupService> _logger;

        public SSECleanupService(IServiceProvider serviceProvider, ILogger<SSECleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio de limpieza SSE iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Cada 5 minutos

                    using var scope = _serviceProvider.CreateScope();
                    var taskEventService = scope.ServiceProvider.GetRequiredService<TaskEventService>();
                    taskEventService.CleanupOldTasks();

                    _logger.LogDebug("Limpieza de tareas SSE completada");
                }
                catch (OperationCanceledException)
                {
                    // Servicio detenido
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en el servicio de limpieza SSE");
                }
            }
        }
    }
}