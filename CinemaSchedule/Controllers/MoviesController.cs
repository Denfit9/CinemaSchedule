using CinemaSchedule.Areas.Identity.Data;
using CinemaSchedule.Data;
using CinemaSchedule.Models;
using CinemaSchedule.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

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
        public async Task<IActionResult> Movies(Guid Id, string term = "", int currentPage = 1, DateTime? endsAt = null, DateTime? startsAt=null, string orderBy = "")
        {
            term = string.IsNullOrEmpty(term) ? "" : term.ToLower();
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

            var allMovies = (from u in dbContext.Movies
                             where term == "" || u.MovieName.ToLower().Contains(term)
                             select new Movie
                             {
                                 Id = u.Id,
                                 MovieName = u.MovieName,
                                 MovieDescription = u.MovieDescription,
                                 Duration = u.Duration,
                                 AgeRestriction = u.AgeRestriction,
                                 CinemaId = u.CinemaId,
                                 StartsAt = u.StartsAt,
                                 EndsAt = u.EndsAt,
                             }
                        ).Where(x => x.CinemaId == Id.ToString()).ToList();
            if(startsAt is not null)
            {
                allMovies = allMovies.Where(x=>x.StartsAt >= startsAt).ToList();
            }
            if(endsAt is not null)
            {
                allMovies = allMovies.Where(x => x.EndsAt <= endsAt).ToList();
            }
            var moviesData = new MoviesViewModel();
            moviesData.sortByStart = string.IsNullOrEmpty(orderBy) ? "start_desc" : "";
            moviesData.sortByEnd = orderBy == "end" ? "end_desc" : "end";
            switch (orderBy)
            {
                case "start_desc":
                    allMovies = allMovies.OrderByDescending(x => x.StartsAt).ToList();
                    break;
                case "end_desc":
                    allMovies = allMovies.OrderByDescending(x => x.EndsAt).ToList();
                    break;
                case "end":
                    allMovies = allMovies.OrderBy(x => x.EndsAt).ToList();
                    break;
                default:
                    allMovies = allMovies.OrderBy(x => x.StartsAt).ToList();
                    break;
            }
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



            moviesData.Movies = allMoviesModel.AsQueryable();
            var totalRecords = moviesData.Movies.Count();
            int pageSize = 6;
            var totalPages = (int)Math.Ceiling((decimal)totalRecords / pageSize);
            if (currentPage > totalPages || currentPage < totalPages)
            {
                currentPage = 1;
            }
            moviesData.Movies = moviesData.Movies.Skip((currentPage - 1) * pageSize).Take(pageSize);
            moviesData.Movies = moviesData.Movies;
            moviesData.CurrentPage = currentPage;
            moviesData.TotalPages = totalPages;
            moviesData.PageSize = pageSize;
            moviesData.Term = term;

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
            if (user.CanWrite != true)
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
            if(viewModel.checksGenres is not null)
            {
                foreach (var genreName in viewModel.checksGenres)
                {
                    var genre = allGenres.FirstOrDefault(g => g.GenreName == genreName);

                    if (genre != null)
                    {
                        genreIds.Add(genre.Id);
                    }
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

        [HttpGet]
        public async Task<IActionResult> EditMovie(Guid id)
        {
            var movie = await dbContext.Movies.FindAsync(id);
            User user = await userManager.GetUserAsync(User);
            TempData["Cinema"] = TempData["Cinema"];
            if (movie.CinemaId != user.cinemaID)
            {
                return RedirectToAction("Movies", "Movies");
            }
            if (user.CanRead != true)
            {
                return RedirectToAction("CinemaProfile", "Cinema");
            }
            if (user.CanWrite != true)
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

            var allMovies = dbContext.Movies.Where(x => x.CinemaId == user.cinemaID.ToString()).ToList();
            var exactMovie = allMovies.FirstOrDefault(x => x.Id == id);

            var allGenresConn = dbContext.GenreConnectors.ToList();
            var allGenres = dbContext.Genres.ToList();
            var allCountriesConn = dbContext.CountryConnectors.ToList();
            var allCountries = dbContext.Countries.ToList();

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

            MovieViewModel exactMovieVM = new MovieViewModel
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
            };
            TempData["Cinema"] = TempData["Cinema"];

            return View(exactMovieVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditMovie(MovieViewModel movieViewModel)
        {
            TempData["Cinema"] = TempData["Cinema"];
            bool error = false;
            User user = await userManager.GetUserAsync(User);
            if (user.CanRead != true)
            {
                return RedirectToAction("CinemaProfile", "Cinema");
            }
            if (user.CanWrite != true)
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
            if (movieViewModel.MovieName.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(movieViewModel.MovieName), "Название фильма должно быть заполнено хотя бы одним символом");
                error = true;
            }

            if (movieViewModel.MovieDescription.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(movieViewModel.MovieDescription), "Описание фильма должно быть заполнено хотя бы одним символом");
                error = true;
            }
            if (movieViewModel.checksCountries.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(movieViewModel.checksCountries), "Необходимо выбрать хотя бы одну страну или \"Другое\"");
                error = true;
            }
            if (movieViewModel.AgeRestriction == null)
            {
                ModelState.AddModelError(nameof(movieViewModel.AgeRestriction), "Необходимо выбрать возрастное ограничение");
                error = true;
            }
            if (movieViewModel.StartsAt == DateTime.MinValue || movieViewModel.StartsAt == null)
            {
                ModelState.AddModelError(nameof(movieViewModel.StartsAt), "Необходимо выбрать дату начала проката");
                error = true;
            }
            if (movieViewModel.EndsAt == DateTime.MinValue || movieViewModel.EndsAt == null)
            {
                ModelState.AddModelError(nameof(movieViewModel.EndsAt), "Необходимо выбрать дату конца проката");
                error = true;
            }
            if (movieViewModel.EndsAt <= movieViewModel.StartsAt)
            {
                ModelState.AddModelError(nameof(movieViewModel.EndsAt), "Конец проката должен быть позже его начала");
                error = true;
            }
            if (movieViewModel.Hours == 0 && movieViewModel.Minutes == 0)
            {
                ModelState.AddModelError(nameof(movieViewModel.Minutes), "Минимальная продолжительность фильма 1 минута");
                error = true;
            }
            TempData["Cinema"] = TempData["Cinema"];
            if (error)
            {
                return View(movieViewModel);
            }
            movieViewModel.Duration = movieViewModel.Hours * 3600 + movieViewModel.Minutes * 60;

            var movie = await dbContext.Movies.FindAsync(movieViewModel.Id);
            if(movie is not null)
            {
                movie.MovieName = movieViewModel.MovieName;
                movie.MovieDescription = movieViewModel.MovieDescription;
                movie.Duration = movieViewModel.Duration;
                movie.CinemaId = user.cinemaID;
                movie.AgeRestriction = movieViewModel.AgeRestriction;
                movie.StartsAt = movieViewModel.StartsAt;
                movie.EndsAt = movieViewModel.EndsAt;
                await dbContext.SaveChangesAsync();
            }

            var movieID = movie.Id;

            var allGenres = dbContext.Genres.ToList();
            var genreIds = new List<Guid>();
            if (movieViewModel.checksGenres is not null)
            {
                foreach (var genreName in movieViewModel.checksGenres)
                {
                    var genre = allGenres.FirstOrDefault(g => g.GenreName == genreName);

                    if (genre != null)
                    {
                        genreIds.Add(genre.Id);
                    }
                }
            }


            var allCountries = dbContext.Countries.ToList();
            var countryIds = new List<Guid>();

            foreach (var countryName in movieViewModel.checksCountries)
            {
                var country = allCountries.FirstOrDefault(g => g.CountryName == countryName);

                if (country != null)
                {
                    countryIds.Add(country.Id);
                }
            }

            var genreConnectorsToDelete = dbContext.GenreConnectors.Where(gc => gc.MovieId == movie.Id.ToString());

            dbContext.GenreConnectors.RemoveRange(genreConnectorsToDelete);
            dbContext.SaveChanges();

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

            var countryConnectorsToDelete = dbContext.CountryConnectors.Where(gc => gc.MovieId == movie.Id.ToString());

            dbContext.CountryConnectors.RemoveRange(countryConnectorsToDelete);
            dbContext.SaveChanges();

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
        [HttpPost]
        public async Task<IActionResult> Delete(MovieViewModel movieViewModel)
        {
            TempData["Cinema"] = TempData["Cinema"];
            User user = await userManager.GetUserAsync(User);
            if (user.CanRead != true)
            {
                return RedirectToAction("CinemaProfile", "Cinema");
            }
            if (user.CanWrite != true)
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

            var movie = await dbContext.Movies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == movieViewModel.Id);

            if (movie is not null)
            {

                dbContext.Movies.Remove(movie);
                await dbContext.SaveChangesAsync();
                var genreConnectorsToDelete = dbContext.GenreConnectors.Where(gc => gc.MovieId == movie.Id.ToString());

                dbContext.GenreConnectors.RemoveRange(genreConnectorsToDelete);
                dbContext.SaveChanges();
                var countryConnectorsToDelete = dbContext.CountryConnectors.Where(gc => gc.MovieId == movie.Id.ToString());

                dbContext.CountryConnectors.RemoveRange(countryConnectorsToDelete);
                dbContext.SaveChanges();
            }

            return RedirectToAction("Movies", "Movies");
        }
    }
}
