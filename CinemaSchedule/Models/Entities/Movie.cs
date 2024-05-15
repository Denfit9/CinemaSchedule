namespace CinemaSchedule.Models.Entities
{
    public class Movie
    {
        public Guid Id { get; set; }
        public string MovieName { get; set; }
        public string MovieDescription { get; set; }
        public int AgeRestriction { get; set; }
        public string CinemaId { get; set; }
        public int Duration { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
    }
}
