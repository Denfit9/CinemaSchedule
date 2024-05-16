using Microsoft.AspNetCore.Mvc;

namespace CinemaSchedule.Controllers
{
    public class ScheduleController : Controller
    {
        public IActionResult Schedule()
        {
            return View();
        }
    }
}
