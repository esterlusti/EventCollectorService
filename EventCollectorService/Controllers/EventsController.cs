using Microsoft.AspNetCore.Mvc;
using EventCollectorService.Services;

namespace EventCollectorServices.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly IQueueService _queueService;

        public EventsController(IQueueService queueService)
        {
            _queueService = queueService;
        }

        [HttpGet]
        public IActionResult GetRecentEvents()
        {
            var events = _queueService.GetRecentEvents();
            return Ok(events);
        }
    }
}

