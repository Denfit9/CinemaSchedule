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
        private readonly DBContext _dbContext;
        public MembersController(UserManager<User> userManager, DBContext dBContext)
        {
            _userManager = userManager;
            _dbContext = dBContext;
        }

        [Route("Members")]
        public IActionResult Members(string term="", int currentPage=1)
        {
            term = string.IsNullOrEmpty(term)?"": term.ToLower();
            var usersData = new UsersListVM();
            var users = (from u in _dbContext.Users
                         where term=="" || u.FirstName.ToLower().Contains(term) || u.LastName.ToLower().Contains(term) || u.Id.ToLower().Contains(term)
                         select new User
                         {
                             Id = u.Id,
                             FirstName = u.FirstName,
                             LastName = u.LastName,
                             Email = u.Email,
                             ProfilePicture = u.ProfilePicture,
                             cinemaID = u.cinemaID,
                         }
                        ).Where(x => x.Id != _userManager.GetUserId(User));

            var totalRecords = users.Count();
            int pageSize = 6;
            var totalPages = (int)Math.Ceiling((decimal)totalRecords / pageSize);
            if (currentPage > totalPages || currentPage<totalPages)
            {
                currentPage = 1;
            }
            users = users.Skip((currentPage-1)*pageSize).Take(pageSize);
            usersData.Users = users;
            usersData.CurrentPage=currentPage;
            usersData.TotalPages=totalPages;
            usersData.PageSize = pageSize;
            usersData.Term = term;
            return View(usersData);
        }
        [HttpGet]
        public async Task<IActionResult> UserProfile(Guid id)
        {
            User user = await _userManager.FindByIdAsync(id.ToString());
            if (id.ToString() == _userManager.GetUserId(User))
            {
                return RedirectToAction("Profile", "Profile");
            }
            {
                return View(user);
            }
        }
    }
}
