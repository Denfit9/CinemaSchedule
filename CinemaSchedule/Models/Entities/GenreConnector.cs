using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaSchedule.Models.Entities
{
    public class GenreConnector
    {
        public Guid GenreConnectorId { get; set; }
        public string GenreId { get; set; }
        public string MovieId { get; set; }
    }
}
