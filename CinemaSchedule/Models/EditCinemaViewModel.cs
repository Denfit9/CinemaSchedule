namespace CinemaSchedule.Models
{
    public class EditCinemaViewModel
    {
        public string CinemaName { get; set; }
        public string CinemaDescription { get; set; }
        public string CinemaAddress { get; set; }
        public string DirectorId { get; set; }
        public IFormFile CinemaPicture { get; set; }
    }
}
