using EventCollectorService.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace EventCollectorService.Background
{
    public class EventBackgroundWorker : BackgroundService
    {
        private readonly IEventsCollectorService _eventCollector;
        private readonly IQueueService _queueService;
        private readonly ILogger<EventBackgroundWorker> _logger;

        public EventBackgroundWorker(IEventsCollectorService eventCollector, IQueueService queueService, ILogger<EventBackgroundWorker> logger)
        {
            _eventCollector = eventCollector;
            _queueService = queueService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Collecting events...");
                var events = _eventCollector.CollectEvents();
                foreach (var evt in events)
                {
                    _queueService.SendEvent(evt);
                }
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}


