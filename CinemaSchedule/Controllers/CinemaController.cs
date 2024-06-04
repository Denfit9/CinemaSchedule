using CinemaSchedule.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CinemaSchedule.Data;
using CinemaSchedule.Models;
using CinemaSchedule.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace CinemaSchedule.Controllers
{
    public class CinemaController : Controller
    {

        private readonly DBContext dbContext;
        private readonly UserManager<User> userManager;
        public CinemaController(DBContext dBContext, UserManager<User> userManager)
        {
            this.dbContext = dBContext;
            this.userManager = userManager;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CinemaProfile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user.cinemaID is not null)
            {
                var cinema = await dbContext.Cinemas.FindAsync(new Guid(user.cinemaID));
                int hallsNumber = 0;
                foreach(var hall in dbContext.Halls)
                {
                    if (new Guid(hall.CinemaId) == cinema.Id)
                    {
                        hallsNumber++;
                    }
                }
                TempData["HallsCount"] = hallsNumber;
                return View(cinema);
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddCinema()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCinema(AddCinemaViewModel viewModel)
        {
            var user = await userManager.GetUserAsync(User);
            if (user.cinemaID is not null)
            {
                return RedirectToAction("CinemaProfile", "Cinema");
            }
            if (viewModel.CinemaName.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(viewModel.CinemaName), "Название кинотеатра должно быть заполнено хотя бы одним символом");
                return View(viewModel);
            }

            if (viewModel.CinemaAddress.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(viewModel.CinemaAddress), "Адрес кинотеатра должно быть заполнено хотя бы одним символом");
                return View(viewModel);
            }

            if (viewModel.CinemaDescription.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(viewModel.CinemaDescription), "Описание кинотеатра должно быть заполнено хотя бы одним символом");
                return View(viewModel);
            }
            var cinema = new Cinema()
            {
                CinemaName = viewModel.CinemaName,
                CinemaAddress = viewModel.CinemaAddress,
                CinemaDescription = viewModel.CinemaDescription,
                DirectorId = userManager.GetUserId(User),
            };
            await dbContext.Cinemas.AddAsync(cinema);
            await dbContext.SaveChangesAsync();
            if (userManager.GetUserId(User) is not null)
            {
                user.cinemaID = dbContext.Cinemas.FirstOrDefault(x => x.DirectorId == userManager.GetUserId(User)).Id.ToString();
                user.CanRead = true;
                user.CanWrite = true;  
                var result = await userManager.UpdateAsync(user);
            }

            return RedirectToAction("CinemaProfile", "Cinema");
        }

        [HttpGet]
        [Authorize]
        public async Task <IActionResult> EditCinema(Guid Id)
        {
            var cinema = await dbContext.Cinemas.FindAsync(Id);
            if (cinema.DirectorId != userManager.GetUserId(User))
            {
                return RedirectToAction("CinemaProfile", "Cinema");
            }
            return View(cinema);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditCinema(Cinema cinemaViewModel)
        {
            var cinema = await dbContext.Cinemas.FindAsync(cinemaViewModel.Id);
            if (cinema.DirectorId != userManager.GetUserId(User))
            {
                return RedirectToAction("CinemaProfile", "Cinema");
            }

            var cinemaPictureFile = Request.Form.Files["CinemaPicture"];

            if (cinemaPictureFile != null && cinemaPictureFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await cinemaPictureFile.CopyToAsync(memoryStream);

                    // Upload the file if less than 2 MB  
                    if (memoryStream.Length < 2097152)
                    {
                        cinema.CinemaPicture = memoryStream.ToArray();
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Файл слишком большой");
                    }
                }
            }
            if (cinemaViewModel.CinemaName.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(cinemaViewModel.CinemaName), "Название кинотеатра должно быть заполнено хотя бы одним символом");
                return View(cinemaViewModel);
            }
            if (cinemaViewModel.CinemaAddress.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(cinemaViewModel.CinemaAddress), "Адрес кинотеатра должен быть заполнен хотя бы одним символом");
                return View(cinemaViewModel);
            }
            if (cinemaViewModel.CinemaDescription.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(cinemaViewModel.CinemaDescription), "Описание кинотеатра должно быть заполнено хотя бы одним символом");
                return View(cinemaViewModel);
            }

            if (cinema is not null)
            {
                cinema.CinemaName = cinemaViewModel.CinemaName;
                cinema.CinemaAddress = cinemaViewModel.CinemaAddress;
                cinema.CinemaDescription = cinemaViewModel.CinemaDescription;

                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("CinemaProfile", "Cinema");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditCinemaEmployee(Guid Id)
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> CinemaProfileVisit(Guid id)
        {
            User user = await userManager.GetUserAsync(User);
            User userEmployee = await userManager.FindByIdAsync(id.ToString());
            Cinema cinema = dbContext.Cinemas.FirstOrDefault(x => x.Id == new Guid(userEmployee.cinemaID));
            if (cinema.Id.ToString() == user.cinemaID)
            {
                return RedirectToAction("CinemaProfile", "Cinema");
            }
            {
                int hallsNumber = 0;
                foreach (var hall in dbContext.Halls)
                {
                    if (new Guid(hall.CinemaId) == cinema.Id)
                    {
                        hallsNumber++;
                    }
                }
                TempData["HallsCount"] = hallsNumber;
                return View(cinema);
            }
        }

    }
}
