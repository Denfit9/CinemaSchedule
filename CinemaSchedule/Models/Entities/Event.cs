namespace CinemaSchedule.Models.Entities
{
    public class Event
    {
        public Guid Id { get; set; }
        public string? EventName { get; set; }
        public string HallId { get; set; }
        public DateTime Begins { get; set; }
        public DateTime Ends { get; set; }
        public int? Duration { get; set; }
        public string Type { get; set; }
        public string MovieId { get; set; }
    }
}
