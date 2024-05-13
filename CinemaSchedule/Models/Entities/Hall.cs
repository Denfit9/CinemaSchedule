namespace CinemaSchedule.Models.Entities
{
    public class Hall
    {
        public Guid Id { get; set; }
        public string HallName { get; set; }
        public string HallDescription { get; set; }
        public string CinemaId { get; set; }
    }
}
