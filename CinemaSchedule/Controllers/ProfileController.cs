using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSchedule.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        [Route("Profile")]
        public IActionResult Profile()
        {
            return View();
        }


    }
}
