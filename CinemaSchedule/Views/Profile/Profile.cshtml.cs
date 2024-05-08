using CinemaSchedule.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace CinemaSchedule.Views.Profile
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public User? applicationUser;
        public ProfileModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public void OnGet()
        {
            var task = _userManager.GetUserAsync(User);
            task.Wait();
            applicationUser = task.Result;
        }
    }
}
