using System.ComponentModel.DataAnnotations;

namespace CinemaSchedule.Models.Entities
{
    public class Genre
    {
        public Guid Id { get; set; }
        public string GenreName { get; set; }
    }
}
