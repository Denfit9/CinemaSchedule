namespace CinemaSchedule.Models.Entities
{
    public class Note
    {
        public Guid Id { get; set; }
        public string NoteName { get; set; }
        public string NoteDescription { get; set; }
        public string UserId { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
