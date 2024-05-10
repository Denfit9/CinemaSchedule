using CinemaSchedule.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSchedule.Controllers
{
    public class UserProfileController : Controller
    {
        [Authorize]
        [Route ("Members/User")]
        public IActionResult UserProfile(User user)
        {
            return View();
        }
    }
}
