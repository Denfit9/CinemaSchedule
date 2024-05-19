using CinemaSchedule.Models.Entities;

namespace CinemaSchedule.Models
{
    public class AddAutoSchedule
    {
            public string? HallId { get; set; }
            public DateTime Date { get; set; }
            public int? Duration { get; set; }
            public string? ModeSelected { get; set; }
            public int? HoursBreak { get; set; }
            public int? MinutesBreak { get; set; }
            public int? HoursDayBegins { get; set; }
            public int? MinutesDayBegins { get; set; }
            public int? HoursDayEnds { get; set; }
            public int? MinutesDayEnds { get; set; }
            public List<Hall> Halls { get; set; }
            public List<Movie> Movies { get; set; }
            public List<string> moviesSelected { get; set; }
            public List<string> hits { get; set; }
    }
}
