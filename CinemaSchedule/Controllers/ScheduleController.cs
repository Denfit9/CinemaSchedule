using CinemaSchedule.Areas.Identity.Data;
using CinemaSchedule.Data;
using CinemaSchedule.Models;
using CinemaSchedule.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Schedule(DateTime date)
        {
            var user = await userManager.GetUserAsync(User);
            var halls = await dbContext.Halls.Where(x => x.CinemaId == user.cinemaID).ToListAsync();
            var scheduleHalls = new ScheduleViewModel();
            scheduleHalls.HallsSchedules = new List<HallsScheduleViewModel>();
            scheduleHalls.Date = date;



            foreach (var hall in halls)
            {
                var events = await dbContext.Events.Where(x => x.HallId == hall.Id.ToString() && x.Begins.Date == date.Date).OrderBy(x => x.Begins).ToListAsync();
                scheduleHalls.HallsSchedules.Add(new HallsScheduleViewModel
                {
                    Id = hall.Id,
                    HallName = hall.HallName,
                    HallDescription = hall.HallDescription,
                    CinemaId = hall.CinemaId,
                    Events = events.AsQueryable()
                });
            }

            return View(scheduleHalls);
        }

        [HttpGet]
        public async Task<IActionResult> NewEvent(DateTime? date)
        {
            var user = await userManager.GetUserAsync(User);
            var allMovies = dbContext.Movies.Where(x => x.CinemaId == user.cinemaID.ToString()).ToList();
            allMovies = allMovies.Where(x => x.StartsAt <= date).ToList();
            allMovies = allMovies.Where(x => x.EndsAt <= date).ToList();
            var halls = await dbContext.Halls.Where(x => x.CinemaId == user.cinemaID).ToListAsync();
            AddEventViewModel eventViewModel = new AddEventViewModel { Begins = date, Halls = halls, Movies = allMovies };
            return View(eventViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> NewEvent(AddEventViewModel eventViewModel, DateTime? date)
        {
            bool error = false;
            var user = await userManager.GetUserAsync(User);
            var halls = await dbContext.Halls.Where(x => x.CinemaId == user.cinemaID).ToListAsync();
            var allMovies = dbContext.Movies.Where(x => x.CinemaId == user.cinemaID.ToString()).ToList();
            allMovies = allMovies.Where(x => x.StartsAt <= date).ToList();
            allMovies = allMovies.Where(x => x.EndsAt <= date).ToList();
            eventViewModel.Halls = halls;
            eventViewModel.Movies = allMovies;
            if (eventViewModel.Type == "Movie")
            {
                if (eventViewModel.MovieId == null || eventViewModel.MovieId.Length == 0)
                {
                    ModelState.AddModelError(nameof(eventViewModel.MovieId), "Необходимо выбрать фильм");
                    error = true;
                }
                else
                {
                    var movie = await dbContext.Movies.FindAsync(new Guid(eventViewModel.MovieId));
                    if (movie.Duration + eventViewModel.MinutesBegin * 60 + eventViewModel.HoursBegin * 3600 >= 86400)
                    {
                        ModelState.AddModelError(nameof(eventViewModel.HoursBegin), "Фильм должен начинаться и заканчиваться в тот же день, ваш фильм превышает это значение на " + ((movie.Duration + eventViewModel.MinutesBegin * 60 + eventViewModel.HoursBegin * 3600 - 86400) / 60 + 1) + " минут(у)");
                        error = true;
                    }
                    eventViewModel.HoursDuration = movie.Duration / 60;
                    eventViewModel.MinutesDuration = (movie.Duration - eventViewModel.HoursDuration * 3600) / 60;
                }

            }
            else if (eventViewModel.Type == "Break")
            {
                if (eventViewModel.EventName is null || eventViewModel.EventName.Length == 0)
                {
                    ModelState.AddModelError(nameof(eventViewModel.EventName), "Необходимо заполнить название события");
                    error = true;
                }
                if (eventViewModel.HoursDuration == 0 && eventViewModel.MinutesDuration == 0)
                {
                    ModelState.AddModelError(nameof(eventViewModel.Duration), "Минимальная продолжительность события 1 минута");
                    error = true;
                }
                if (eventViewModel.HoursDuration * 3600 + eventViewModel.MinutesDuration * 60 + eventViewModel.MinutesBegin * 60 + eventViewModel.HoursBegin * 3600 >= 86400)
                {
                    ModelState.AddModelError(nameof(eventViewModel.MinutesBegin), "Событие должно начинаться и заканчиваться в тот же день, ваше событие превышает это значение на " + ((eventViewModel.HoursDuration * 3600 + eventViewModel.MinutesDuration * 60 + eventViewModel.MinutesBegin * 60 + eventViewModel.HoursBegin * 3600 - 86400) / 60 + 1) + " минут(у)");
                    error = true;
                }

            }
            if (eventViewModel.HallId == null)
            {
                ModelState.AddModelError(nameof(eventViewModel.HallId), "Необходимо выбрать зал");
                error = true;
            }
            else
            {
                var allEvents = dbContext.Events.Where(x => x.HallId == eventViewModel.HallId.ToString()).ToList();
                allEvents = allEvents.Where(x => x.Begins.Date == date.Value.Date).ToList();
                DateTime dateTime = date ?? DateTime.MinValue;
                var newEventStart = dateTime.AddSeconds((double)(eventViewModel.HoursBegin * 3600 + eventViewModel.MinutesBegin * 60));
                var newEventEnd = dateTime.AddSeconds((double)((double)((eventViewModel.HoursBegin * 3600) + eventViewModel.MinutesBegin * 60) + eventViewModel.HoursDuration * 3600 + eventViewModel.MinutesDuration * 60));

                var overlappingEvent = GetOverlappingEvent(newEventStart, newEventEnd, allEvents);
                if (overlappingEvent == null)
                {

                }
                else
                {
                    if (eventViewModel.Type == "Movie")
                    {
                        ModelState.AddModelError(nameof(eventViewModel.HoursBegin), "Обнаружено пересечение с событием \"" + overlappingEvent.EventName + "\", которое начинается в " + overlappingEvent.Begins.ToString("HH:mm"));
                        error = true;
                    }
                    else if (eventViewModel.Type == "Break")
                    {
                        ModelState.AddModelError(nameof(eventViewModel.MinutesBegin), "Обнаружено пересечение с событием \"" + overlappingEvent.EventName + "\", которое начинается в " + overlappingEvent.Begins.ToString("HH:mm"));
                        error = true;
                    }
                }
            }
            if (error)
            {
                return View(eventViewModel);
            }
            if (eventViewModel.Type == "Movie")
            {
                var movie = await dbContext.Movies.FindAsync(new Guid(eventViewModel.MovieId));
                DateTime dateTime = date ?? DateTime.MinValue;
                var Event = new Event()
                {
                    MovieId = movie.Id.ToString(),
                    EventName = movie.MovieName,
                    Duration = movie.Duration,
                    HallId = eventViewModel.HallId,
                    Type = "Фильм",
                    Begins = dateTime.AddSeconds((double)(eventViewModel.HoursBegin * 3600 + eventViewModel.MinutesBegin * 60)),
                    Ends = dateTime.AddSeconds((double)(eventViewModel.HoursBegin * 3600 + eventViewModel.MinutesBegin * 60) + movie.Duration)
                };
                await dbContext.Events.AddAsync(Event);
                await dbContext.SaveChangesAsync();
            }
            else if (eventViewModel.Type == "Break")
            {
                DateTime dateTime = date ?? DateTime.MinValue;
                var Event = new Event()
                {
                    EventName = eventViewModel.EventName,
                    Duration = eventViewModel.HoursDuration * 3600 + eventViewModel.MinutesDuration * 60,
                    HallId = eventViewModel.HallId,
                    Type = "Перерыв",
                    Begins = dateTime.AddSeconds((double)(eventViewModel.HoursBegin * 3600 + eventViewModel.MinutesBegin * 60)),
                    Ends = dateTime.AddSeconds((double)((double)(eventViewModel.HoursBegin * 3600 + eventViewModel.MinutesBegin * 60) + eventViewModel.HoursDuration * 3600 + eventViewModel.MinutesDuration * 60))
                };
                await dbContext.Events.AddAsync(Event);
                await dbContext.SaveChangesAsync();
            }
            string datePicked = date?.ToString("yyyy-MM-dd") ?? string.Empty;
            return RedirectToAction("Schedule", "Schedule", new { date = datePicked });
        }

        [HttpGet]
        public async Task<IActionResult> EditEvent(Guid id)
        {
            var Event = await dbContext.Events.FindAsync(id);
            EditEventViewModel editEventViewModel = new EditEventViewModel
            {
                Id = id,
                EventName = Event.EventName,
                HallId = Event.HallId,
                Begins = Event.Begins,
                Ends = Event.Ends,
                Duration = Event.Duration,
                Type = Event.Type,
                MovieId = Event.MovieId,
                Hours = Event.Begins.Hour,
                Minutes = Event.Begins.Minute
            };
            return View(editEventViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> EditEvent(EditEventViewModel editEventViewModel)
        {
            var Event = await dbContext.Events.FindAsync(editEventViewModel.Id);
            var allEvents = dbContext.Events.Where(x => x.HallId == Event.HallId.ToString() && x.Id != editEventViewModel.Id).ToList();
            bool error = false;
            if (Event.Duration + editEventViewModel.Minutes * 60 + editEventViewModel.Hours * 3600 >= 86400)
            {
                ModelState.AddModelError(nameof(editEventViewModel.Hours), "Событие должно начинаться и заканчиваться в тот же день, ваше событие превышает это значение на " + ((Event.Duration + editEventViewModel.Minutes * 60 + editEventViewModel.Hours * 3600 - 86400) / 60 + 1) + " минут(у)");
                error = true;
            }
            var newEventStart = Event.Begins.Date.AddSeconds((double)(editEventViewModel.Hours * 3600 + editEventViewModel.Minutes * 60));
            var newEventEnd = Event.Begins.Date.AddSeconds((double)((double)((editEventViewModel.Hours * 3600) + editEventViewModel.Minutes * 60) + Event.Duration));
            var overlappingEvent = GetOverlappingEvent(newEventStart, newEventEnd, allEvents);
            if (overlappingEvent == null)
            {

            }
            else
            {

                ModelState.AddModelError(nameof(editEventViewModel.Hours), "Обнаружено пересечение с событием \"" + overlappingEvent.EventName + "\", которое начинается в " + overlappingEvent.Begins.ToString("HH:mm"));
                error = true;

            }
            if (error)
            {
                return View(editEventViewModel);
            };
            if (Event is not null)
            {
                Event.Begins = newEventStart;
                Event.Ends = newEventEnd;
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Schedule", "Schedule", new { date = Event.Begins.ToString("yyyy-MM-dd") });
        }
        public Event GetOverlappingEvent(DateTime newEventStart, DateTime newEventEnd, List<Event> existingEvents)
        {
            foreach (var existingEvent in existingEvents)
            {
                if (newEventStart < existingEvent.Ends && newEventEnd > existingEvent.Begins)
                {
                    return existingEvent;
                }
            }
            return null;
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var Event = await dbContext.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            string datePicked = Event.Begins.ToString("yyyy-MM-dd");
            if (Event is not null)
            {
                dbContext.Events.Remove(Event);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Schedule", "Schedule", new { date = datePicked });
        }
    }
}
