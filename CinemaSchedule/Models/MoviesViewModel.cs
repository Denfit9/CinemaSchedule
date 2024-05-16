using CinemaSchedule.Areas.Identity.Data;

namespace CinemaSchedule.Models
{
    public class MoviesViewModel
    {
        public IQueryable<MovieViewModel> Movies { get; set; }
        public int? PageSize { get; set; }
        public int? CurrentPage { get; set; }
        public int? TotalPages { get; set; }
        public string? Term { get; set; }
        public DateTime? StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
        public string sortByStart { get; set; }
        public string sortByEnd { get; set;}
    }
}
