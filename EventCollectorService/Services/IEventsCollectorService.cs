using System.Diagnostics;
using EventCollectorService.Models;

namespace EventCollectorService.Services
{
    public interface IEventsCollectorService
    {
        List<EventData> CollectEvents();
    }

}
