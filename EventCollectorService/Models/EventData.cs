namespace EventCollectorService.Models
{
    public class EventData
    {
        public DateTime ReportTime { get; set; }
        public string Source { get; set; }
        public string Content { get; set; }
        public string Level { get; set; }
    }
}
