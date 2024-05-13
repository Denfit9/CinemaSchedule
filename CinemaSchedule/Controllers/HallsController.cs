using CinemaSchedule.Areas.Identity.Data;
using CinemaSchedule.Data;
using CinemaSchedule.Models.Entities;
using CinemaSchedule.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace CinemaSchedule.Controllers
{
    [Authorize]
    public class HallsController : Controller
    {
        private readonly DBContext dbContext;
        private readonly UserManager<User> userManager;
        public HallsController(DBContext dBContext, UserManager<User> userManager)
        {
            this.dbContext = dBContext;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Halls(Guid Id)
        {
            var cinema = await dbContext.Cinemas.FindAsync(Id);
            TempData["CinemaId"] = Id;
            if (cinema.DirectorId != userManager.GetUserId(User))
            {
                return RedirectToAction("CinemaProfile", "Cinema");
            }

            var halls = await dbContext.Halls.Where(x => x.CinemaId == cinema.Id.ToString()).ToListAsync();
            return View(halls);
        }

        [HttpGet]
        public async Task<IActionResult> AddHall(Guid id)
        {
            var cinema = await dbContext.Cinemas.FindAsync(id);
            if (cinema.DirectorId != userManager.GetUserId(User))
            {
                return RedirectToAction("CinemaProfile", "Cinema");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddHall(AddHallViewModel viewModel, Guid id)
        {
            var cinema = await dbContext.Cinemas.FindAsync(id);
            if (cinema.DirectorId != userManager.GetUserId(User))
            {
                return RedirectToAction("CinemaProfile", "Cinema");
            }
            if (viewModel.HallName.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(viewModel.HallName), "Название зала должно быть заполнено хотя бы одним символом");
                return View(viewModel);
            }

            if (viewModel.HallDescription.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(viewModel.HallDescription), "Описание зала должно быть заполнено хотя бы одним символом");
                return View(viewModel);
            }
            var hall = new Hall()
            {
                HallName = viewModel.HallName,
                HallDescription = viewModel.HallDescription,
                CinemaId = id.ToString(),
            };
            await dbContext.Halls.AddAsync(hall);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Halls", "Halls", new {id});
        }

        [HttpGet]
        public async Task<IActionResult> EditHall(Guid id)
        {
            var hall = await dbContext.Halls.FindAsync(id);
            var cinema = await dbContext.Cinemas.FindAsync(new Guid(hall.CinemaId));
            if (cinema.DirectorId != userManager.GetUserId(User))
            {
                return RedirectToAction("Halls", "Halls", new {cinema.Id});
            }
            return View(hall);
        }

        [HttpPost]
        public async Task<IActionResult> EditHall(Hall viewModel)
        {
            var hall = await dbContext.Halls.FindAsync(viewModel.Id);
            var cinema = await dbContext.Cinemas.FindAsync(new Guid(hall.CinemaId));
            if (cinema.DirectorId != userManager.GetUserId(User))
            {
                return RedirectToAction("Halls", "Halls", new { cinema.Id });
            }
            if (viewModel.HallName.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(viewModel.HallName), "Название зала должно быть заполнено хотя бы одним символом");
                return View(viewModel);
            }

            if (viewModel.HallDescription.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(viewModel.HallDescription), "Описание зала должно быть заполнено хотя бы одним символом");
                return View(viewModel);
            }

            if (hall is not null)
            {
                hall.HallName = viewModel.HallName;
                hall.HallDescription = viewModel.HallDescription;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Halls", "Halls", new { cinema.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Hall viewModel)
        {

            var hall = await dbContext.Halls
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == viewModel.Id);
            if (hall is not null)
            {
                var cinema = await dbContext.Cinemas.FindAsync(new Guid(hall.CinemaId));
                if (cinema.DirectorId != userManager.GetUserId(User))
                {
                    return RedirectToAction("CinemaProfile", "Cinema");
                }
                dbContext.Halls.Remove(viewModel);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Halls", "Halls", new { cinema.Id });
            }
            else
            {
                return RedirectToAction("CinemaProfile", "Cinema");
            }
        }
    }
}
