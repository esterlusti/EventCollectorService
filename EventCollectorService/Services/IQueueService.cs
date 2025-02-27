using EventCollectorService.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Experimental.System.Messaging;

namespace EventCollectorService.Services
{
    public interface IQueueService
    {
        void SendEvent(EventData eventData);
        List<EventData> GetRecentEvents(int count = 10);
    }
}
