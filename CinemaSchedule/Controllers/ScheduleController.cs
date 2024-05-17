using CinemaSchedule.Areas.Identity.Data;
using CinemaSchedule.Data;
using CinemaSchedule.Models;
using CinemaSchedule.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace CinemaSchedule.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {


        private readonly DBContext dbContext;
        private readonly UserManager<User> userManager;
        public ScheduleController(DBContext dBContext, UserManager<User> userManager)
        {
            this.dbContext = dBContext;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Schedule()
        {
            var user = await userManager.GetUserAsync(User);
            var halls = await dbContext.Halls.Where(x => x.CinemaId == user.cinemaID).ToListAsync();
            var scheduleHalls = new ScheduleViewModel();
            scheduleHalls.HallsSchedules = new List<HallsScheduleViewModel>();

            foreach (var hall in halls)
            {
                scheduleHalls.HallsSchedules.Add(new HallsScheduleViewModel
                {
                    Id = hall.Id,
                    HallName = hall.HallName,
                    HallDescription = hall.HallDescription,
                    CinemaId = hall.CinemaId
                });
            }

            return View(scheduleHalls);
        }

        [HttpGet]
        public async Task<IActionResult> NewEvent(DateTime? date){
            var user = await userManager.GetUserAsync(User);
            var allMovies = dbContext.Movies.Where(x => x.CinemaId == user.cinemaID.ToString()).ToList();
                allMovies = allMovies.Where(x => x.StartsAt <= date).ToList();
                allMovies = allMovies.Where(x => x.EndsAt <= date).ToList();
            var halls = await dbContext.Halls.Where(x => x.CinemaId == user.cinemaID).ToListAsync();
            AddEventViewModel eventViewModel = new AddEventViewModel{ Begins = date, Halls = halls, Movies = allMovies };
            return View(eventViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> NewEvent(AddEventViewModel eventViewModel)
        {

            return RedirectToAction("Schedule", "Schedule");
        }
    }
}
