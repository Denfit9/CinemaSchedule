namespace CinemaSchedule.Models
{
    public class ProfileEditViewModel
    {
        public IFormFile ProfilePicture { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
