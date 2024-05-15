namespace CinemaSchedule.Models.Entities
{
    public class CountryConnector
    {
        public Guid CountryConnectorId { get; set; }
        public string CountryId { get; set; }
        public string MovieId { get; set; }
    }
}
