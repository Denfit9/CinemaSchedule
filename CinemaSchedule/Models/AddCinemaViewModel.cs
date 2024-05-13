namespace CinemaSchedule.Models
{
    public class AddCinemaViewModel
    {
        public string CinemaName { get; set; }
        public string CinemaDescription { get; set; }
        public string CinemaAddress { get; set; }
        public string DirectorId { get; set; }
        public byte[]? CinemaPicture { get; set; }
    }
}
