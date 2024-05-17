using CinemaSchedule.Models.Entities;

namespace CinemaSchedule.Models
{
    public class HallsScheduleViewModel
    {
        public Guid Id { get; set; }
        public string HallName { get; set; }
        public string HallDescription { get; set; }
        public string CinemaId { get; set; }
        public IQueryable<Event>? Events { get; set; }
    }
}
