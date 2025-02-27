using EventCollectorService.Models;
using Experimental.System.Messaging;
using Newtonsoft.Json;

namespace EventCollectorService.Services
{
    public class MsmqQueueService : IQueueService
    {
        private readonly string _queuePath = @".\private$\eventsQueue";
        private readonly ILogger<MsmqQueueService> _logger;

        public MsmqQueueService(ILogger<MsmqQueueService> logger)
        {
            _logger = logger;
            if (!MessageQueue.Exists(_queuePath))
            {
                MessageQueue.Create(_queuePath);
            }
        }

        public void SendEvent(EventData eventData)
        {
            try
            {
                using MessageQueue queue = new(_queuePath);
                var jsonData = JsonConvert.SerializeObject(eventData);
                queue.Send(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending event to MSMQ");
            }
        }

        public List<EventData> GetRecentEvents(int count = 10)
        {
            List<EventData> events = new();
            try
            {
                using MessageQueue queue = new(_queuePath);
                queue.Formatter = new XmlMessageFormatter(new[] { typeof(string) });

                foreach (Message msg in queue.GetAllMessages().Take(count))
                {
                    var eventData = JsonConvert.DeserializeObject<EventData>(msg.Body.ToString());
                    if (eventData != null) events.Add(eventData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving events from MSMQ");
            }

            return events;
        }
    }
}
