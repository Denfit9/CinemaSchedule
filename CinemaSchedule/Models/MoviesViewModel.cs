using CinemaSchedule.Areas.Identity.Data;

namespace CinemaSchedule.Models
{
    public class MoviesViewModel
    {
        public IQueryable<MovieViewModel> Movies { get; set; }
    }
}
