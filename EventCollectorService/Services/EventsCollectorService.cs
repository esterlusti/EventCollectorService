using EventCollectorService.Models;
using System.Diagnostics;

namespace EventCollectorService.Services
{
    public class EventsCollectorService : IEventsCollectorService
    {
        private readonly ILogger<EventsCollectorService> _logger;

        public EventsCollectorService(ILogger<EventsCollectorService> logger)
        {
            _logger = logger;
        }

        public List<EventData> CollectEvents()
        {
            List<EventData> events = new();
            try
            {
                using EventLog eventLog = new("Application");
                var lastHour = DateTime.Now.AddHours(-1);

                foreach (EventLogEntry entry in eventLog.Entries.Cast<EventLogEntry>()
                    .Where(e => e.TimeGenerated >= lastHour))
                {
                    events.Add(new EventData
                    {
                        ReportTime = entry.TimeGenerated,
                        Source = entry.Source,
                        Content = entry.Message,
                        Level = entry.EntryType.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error collecting events from Event Viewer");
            }

            return events;
        }
    }
}
