using CinemaSchedule.Areas.Identity.Data;
using CinemaSchedule.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CinemaSchedule.Controllers
{
    [Authorize]
    public class MembersController : Controller
    {
        private readonly UserManager<User> _userManager;

        public MembersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [Route("Members")]
        public IActionResult Members()
        {
            var users = _userManager.Users.Where(x => x.Id != _userManager.GetUserId(User)).ToList();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile(Guid id)
        {
            User user = await _userManager.FindByIdAsync(id.ToString());
            if(id.ToString() == _userManager.GetUserId(User))
            {
                return RedirectToAction("Profile", "Profile");
            }
            {
                return View(user);
            }
        }
    }
}
