using EventCollectorService.Models;
using Experimental.System.Messaging;
using Newtonsoft.Json;

namespace EventCollectorService.Services
{
    /// <summary>
    /// Service for interacting with MSMQ.
    /// </summary>
    public class MsmqQueueService : IQueueService
    {
        private readonly string _queuePath = @".\private$\eventsQueue";
        private readonly ILogger<MsmqQueueService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsmqQueueService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public MsmqQueueService(ILogger<MsmqQueueService> logger)
        {
            _logger = logger;
            if (!MessageQueue.Exists(_queuePath))
            {
                MessageQueue.Create(_queuePath);
                _logger.LogInformation("Message queue created at path: {QueuePath}", _queuePath);
            }
        }

        /// <summary>
        /// Sends an event to the MSMQ.
        /// </summary>
        /// <param name="eventData">The event data to send.</param>
        public void SendEvent(EventData eventData)
        {
            try
            {
                using MessageQueue queue = new(_queuePath);
                var jsonData = JsonConvert.SerializeObject(eventData);
                queue.Send(jsonData);
                _logger.LogInformation("Event sent to MSMQ: {EventData}", jsonData);
            }
            catch (MessageQueueException mqex)
            {
                _logger.LogError(mqex, "Message Queue error sending event to MSMQ");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending event to MSMQ");
            }
        }

        /// <summary>
        /// Retrieves recent events from the MSMQ.
        /// </summary>
        /// <param name="count">The number of recent events to retrieve.</param>
        /// <returns>A list of recent events.</returns>
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
                _logger.LogInformation("Retrieved {Count} events from MSMQ", events.Count);
            }
            catch (MessageQueueException mqex)
            {
                _logger.LogError(mqex, "Message Queue error retrieving events from MSMQ");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving events from MSMQ");
            }

            return events;
        }
    }
}

