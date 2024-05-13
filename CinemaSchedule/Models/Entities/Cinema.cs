namespace CinemaSchedule.Models.Entities
{
    public class Cinema
    {
        public Guid Id { get; set; }
        public string CinemaName { get; set; }
        public string CinemaDescription { get; set; }
        public string CinemaAddress { get; set; }
        public string DirectorId { get; set; }
        public byte[]? CinemaPicture { get; set; }
    }
}
