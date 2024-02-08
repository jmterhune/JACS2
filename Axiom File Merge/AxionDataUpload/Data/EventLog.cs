namespace AxionDataUpload.Data
{
    public class EventLog
    {
        public int EventID { get; set; }
        public string? EventName { get; set; }
        public string? EventDescription { get; set; }
        public DateTime EventDate { get; set; }
        public string? Source { get; set; }
        public EventType EventType { get; set; }
    }
    public enum EventType
    {
        Event,Error
    }
}

