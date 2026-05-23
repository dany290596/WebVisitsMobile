using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace WebVisitsMobile.Infrastructure.Services
{
    public class TaskEventService
    {
        // Diccionario para guardar canales por taskId
        private readonly ConcurrentDictionary<Guid, Channel<string>> _taskChannels = new();
        private readonly ConcurrentDictionary<Guid, DateTime> _taskCreationTimes = new();
        private readonly ILogger<TaskEventService> _logger;

        public TaskEventService(ILogger<TaskEventService> logger)
        {
            _logger = logger;
        }

        // Registrar una nueva tarea para monitoreo
        public void RegisterTask(Guid taskId)
        {
            _taskChannels.TryAdd(taskId, Channel.CreateUnbounded<string>());
            _taskCreationTimes.TryAdd(taskId, DateTime.UtcNow);
            _logger.LogInformation($"Tarea registrada para SSE: {taskId}");
        }

        // Enviar actualización a todos los clientes suscritos a esta tarea
        public async Task SendUpdateAsync(Guid taskId, string message)
        {
            if (_taskChannels.TryGetValue(taskId, out var channel))
            {
                await channel.Writer.WriteAsync(message);
                _logger.LogDebug($"Update enviado para tarea {taskId}: {message}");
            }
        }

        // Notificar que la tarea está completa
        public async Task NotifyCompletionAsync(Guid taskId, string resultJson)
        {
            var message = $"event: completed\ndata: {resultJson}\n\n";
            await SendUpdateAsync(taskId, message);

            // Opcional: limpiar después de un tiempo
            _ = Task.Delay(TimeSpan.FromMinutes(5)).ContinueWith(_ => CleanupTask(taskId));
        }

        // Notificar error
        public async Task NotifyErrorAsync(Guid taskId, string errorMessage)
        {
            var message = $"event: error\ndata: {errorMessage}\n\n";
            await SendUpdateAsync(taskId, message);
        }

        // Notificar que la tarea está en progreso
        public async Task NotifyProgressAsync(Guid taskId, int progress, string status)
        {
            var progressData = new
            {
                progress,
                status,
                taskId
            };
            var json = System.Text.Json.JsonSerializer.Serialize(progressData);
            var message = $"event: progress\ndata: {json}\n\n";
            await SendUpdateAsync(taskId, message);
        }

        // Obtener el stream de eventos para una tarea
        public async IAsyncEnumerable<string> GetTaskStream(Guid taskId, CancellationToken cancellationToken)
        {
            if (!_taskChannels.TryGetValue(taskId, out var channel))
            {
                yield return $"event: error\ndata: Tarea no encontrada\n\n";
                yield break;
            }

            // Enviar mensaje de conexión establecida
            yield return $"event: connected\ndata: {{\"taskId\":\"{taskId}\",\"timestamp\":\"{DateTime.UtcNow:O}\"}}\n\n";

            // Mantener conexión viva con mensajes de keep-alive
            var keepAliveTask = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(30000, cancellationToken); // Cada 30 segundos
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await channel.Writer.WriteAsync($"event: keepalive\ndata: {{\"timestamp\":\"{DateTime.UtcNow:O}\"}}\n\n");
                    }
                }
            }, cancellationToken);

            // Leer del canal
            await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return message;
            }

            // Si sale del loop, enviar mensaje de desconexión
            yield return $"event: disconnected\ndata: {{\"taskId\":\"{taskId}\",\"timestamp\":\"{DateTime.UtcNow:O}\"}}\n\n";
        }

        // Verificar si una tarea existe
        public bool TaskExists(Guid taskId)
        {
            return _taskChannels.ContainsKey(taskId);
        }

        // Limpiar tarea antigua
        private void CleanupTask(Guid taskId)
        {
            if (_taskChannels.TryRemove(taskId, out var channel))
            {
                channel.Writer.TryComplete();
            }
            _taskCreationTimes.TryRemove(taskId, out _);
            _logger.LogInformation($"Tarea limpiada: {taskId}");
        }

        // Limpiar tareas antiguas (más de 1 hora)
        public void CleanupOldTasks()
        {
            var cutoff = DateTime.UtcNow.AddHours(-1);
            foreach (var kvp in _taskCreationTimes)
            {
                if (kvp.Value < cutoff)
                {
                    CleanupTask(kvp.Key);
                }
            }
        }
    }
}