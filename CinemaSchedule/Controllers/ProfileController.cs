using Microsoft.AspNetCore.Mvc;

namespace CinemaSchedule.Controllers
{
    public class ProfileController : Controller
    {
        [Route("Profile")]
        public IActionResult Profile()
        {
            return View();
        }
    }
}
