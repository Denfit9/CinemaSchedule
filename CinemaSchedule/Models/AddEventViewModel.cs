using CinemaSchedule.Models.Entities;

namespace CinemaSchedule.Models
{
    public class AddEventViewModel
    {
        public string? EventName { get; set; }
        public string? HallId { get; set; }
        public DateTime? Begins { get; set; }
        public DateTime? Ends { get; set; }
        public int? Duration { get; set; }
        public string? Type { get; set; }
        public string? MovieId { get; set; }
        public List<Hall> Halls {get; set;}
        public List<Movie> Movies { get; set; }
        public string? SelectedType { get; set; }
    }
}
