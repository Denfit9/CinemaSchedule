using CinemaSchedule.Areas.Identity.Data;
using CinemaSchedule.Data;
using CinemaSchedule.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Drawing.Printing;

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
        [HttpGet]
        public IActionResult Members(int currentPage)
        {
            if (currentPage == 0)
            {
                currentPage++;
            }
            var users = _userManager.Users.Where(x => x.Id != _userManager.GetUserId(User)).ToList();
            var totalCount = users.Count();
            var totalPages = (int)Math.Ceiling((decimal)totalCount / 6);
            if (currentPage > totalPages)
            {
                ViewData["CurrentPage"] = 1;
                return View(GetMembers(1));
            }
            else
            {
                ViewData["CurrentPage"] = currentPage;
                return View(GetMembers(currentPage));
            }
        }

        public IEnumerable<User> GetMembers(int page = 1, int pageSize=6)
        {
            var users = _userManager.Users.Where(x => x.Id != _userManager.GetUserId(User)).ToList();
            var totalCount = users.Count();
            var totalPages = (int)Math.Ceiling((decimal)totalCount/pageSize);
            var usersPerPage = users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return usersPerPage;
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
