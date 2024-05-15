using CinemaSchedule.Areas.Identity.Data;
using CinemaSchedule.Data;
using CinemaSchedule.Models;
using CinemaSchedule.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CinemaSchedule.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {

        private readonly DBContext dbContext;
        private readonly UserManager<User> userManager;
        public MoviesController(DBContext dBContext, UserManager<User> userManager)
        {
            this.dbContext = dBContext;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Movies(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                TempData["Cinema"] = TempData["Cinema"];
                Id = (Guid)TempData["Cinema"];
            }
            User user = await userManager.GetUserAsync(User);
            if (user.CanRead != true)
            {
                return RedirectToAction("CinemaProfile", "Cinema");
            }
            if (user.cinemaID != Id.ToString())
            {
                return RedirectToAction("CinemaProfile", "Cinema");
            }
            TempData["Cinema"] = Id;

            var allMovies = dbContext.Movies.Where(x => x.CinemaId == Id.ToString()).ToList();
            var allMoviesModel = new List<MovieViewModel>();
            var allGenresConn = dbContext.GenreConnectors.ToList();
            var allGenres = dbContext.Genres.ToList();
            var allCountriesConn = dbContext.CountryConnectors.ToList();
            var allCountries = dbContext.Countries.ToList();
            foreach (var movie in allMovies)
            {
                var genres = new List<string>();
                var countries = new List<string>();

                var genreIds = allGenresConn
                    .Where(g => g.MovieId == movie.Id.ToString())
                    .Select(g => new Guid(g.GenreId))
                    .ToList();

                var countryIds = allCountriesConn
                    .Where(c => c.MovieId == movie.Id.ToString())
                    .Select(c => new Guid(c.CountryId))
                    .ToList();

                foreach (var genreId in genreIds)
                {
                    var genreName = allGenres.FirstOrDefault(g => g.Id == genreId)?.GenreName;
                    if (genreName != null)
                    {
                        genres.Add(genreName);
                    }
                }

                foreach (var countryId in countryIds)
                {
                    var countryName = allCountries.FirstOrDefault(c => c.Id == countryId)?.CountryName;
                    if (countryName != null)
                    {
                        countries.Add(countryName);
                    }
                }

                var durationHours = movie.Duration / 3600;
                var durationMinutes = (movie.Duration % 3600) / 60;

                allMoviesModel.Add(new MovieViewModel
                {
                    Id = movie.Id,
                    MovieName = movie.MovieName,
                    MovieDescription = movie.MovieDescription,
                    CinemaId = movie.CinemaId,
                    Duration = movie.Duration,
                    Hours = durationHours,
                    Minutes = durationMinutes,
                    AgeRestriction = movie.AgeRestriction,
                    StartsAt = movie.StartsAt,
                    EndsAt = movie.EndsAt,
                    checksGenres = genres,
                    checksCountries = countries
                });
            }


            var moviesData = new MoviesViewModel();
            moviesData.Movies = allMoviesModel.AsQueryable();

            return View(moviesData);
        }

        [HttpGet]
        public async Task<IActionResult> AddMovie()
        {
            User user = await userManager.GetUserAsync(User);
            if (user.CanRead != true)
            {
                return RedirectToAction("CinemaProfile", "Cinema");
            }
            if (TempData["Cinema"].ToString() != null)
            {
                if (user.cinemaID != TempData["Cinema"].ToString())
                {
                    return RedirectToAction("CinemaProfile", "Cinema");
                }
            }
            else
            {
                return RedirectToAction("CinemaProfile", "Cinema");
            }
            TempData["Cinema"] = TempData["Cinema"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMovie(AddMovieViewModel viewModel)
        {
            bool error = false;
            User user = await userManager.GetUserAsync(User);
            if (user.CanRead != true)
            {
                return RedirectToAction("CinemaProfile", "Cinema");
            }
            if (TempData["Cinema"].ToString() != null)
            {
                if (user.cinemaID != TempData["Cinema"].ToString())
                {
                    return RedirectToAction("CinemaProfile", "Cinema");
                }
            }
            else
            {
                return RedirectToAction("CinemaProfile", "Cinema");
            }
            if (viewModel.MovieName.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(viewModel.MovieName), "Название фильма должно быть заполнено хотя бы одним символом");
                error = true;
            }

            if (viewModel.MovieDescription.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(viewModel.MovieDescription), "Описание фильма должно быть заполнено хотя бы одним символом");
                error = true;
            }
            if (viewModel.checksCountries.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(viewModel.checksCountries), "Необходимо выбрать хотя бы одну страну или \"Другое\"");
                error = true;
            }
            if (viewModel.AgeRestriction == null)
            {
                ModelState.AddModelError(nameof(viewModel.AgeRestriction), "Необходимо выбрать возрастное ограничение");
                error = true;
            }
            if (viewModel.StartsAt == DateTime.MinValue || viewModel.StartsAt == null)
            {
                ModelState.AddModelError(nameof(viewModel.StartsAt), "Необходимо выбрать дату начала проката");
                error = true;
            }
            if (viewModel.EndsAt == DateTime.MinValue || viewModel.EndsAt == null)
            {
                ModelState.AddModelError(nameof(viewModel.EndsAt), "Необходимо выбрать дату конца проката");
                error = true;
            }
            if (viewModel.EndsAt <= viewModel.StartsAt)
            {
                ModelState.AddModelError(nameof(viewModel.EndsAt), "Конец проката должен быть позже его начала");
                error = true;
            }
            if (viewModel.Hours == 0 && viewModel.Minutes == 0)
            {
                ModelState.AddModelError(nameof(viewModel.Minutes), "Минимальная продолжительность фильма 1 минута");
                error = true;
            }
            TempData["Cinema"] = TempData["Cinema"];
            if (error)
            {
                return View(viewModel);
            }
            viewModel.Duration = viewModel.Hours * 3600 + viewModel.Minutes * 60;
            var movie = new Movie()
            {
                MovieName = viewModel.MovieName,
                MovieDescription = viewModel.MovieDescription,
                Duration = viewModel.Duration,
                CinemaId = user.cinemaID,
                AgeRestriction = viewModel.AgeRestriction,
                StartsAt = viewModel.StartsAt ?? DateTime.Now,
                EndsAt = viewModel.EndsAt ?? DateTime.Now
            };
            await dbContext.Movies.AddAsync(movie);
            await dbContext.SaveChangesAsync();

            var movieID = movie.Id;

            var allGenres = dbContext.Genres.ToList();
            var genreIds = new List<Guid>();

            foreach (var genreName in viewModel.checksGenres)
            {
                var genre = allGenres.FirstOrDefault(g => g.GenreName == genreName);

                if (genre != null)
                {
                    genreIds.Add(genre.Id);
                }
            }

            var allCountries = dbContext.Countries.ToList();
            var countryIds = new List<Guid>();

            foreach (var countryName in viewModel.checksCountries)
            {
                var country = allCountries.FirstOrDefault(g => g.CountryName == countryName);

                if (country != null)
                {
                    countryIds.Add(country.Id);
                }
            }

            foreach (var genreId in genreIds)
            {
                var genreConnector = new GenreConnector
                {
                    MovieId = movieID.ToString(),
                    GenreId = genreId.ToString()
                };

                await dbContext.GenreConnectors.AddAsync(genreConnector);
            }
            await dbContext.SaveChangesAsync();

            foreach (var countryId in countryIds)
            {
                var countryConnector = new CountryConnector
                {
                    MovieId = movieID.ToString(),
                    CountryId = countryId.ToString()
                };

                await dbContext.CountryConnectors.AddAsync(countryConnector);
            }
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Movies", "Movies");
        }
    }
}
