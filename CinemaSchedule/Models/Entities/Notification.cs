namespace CinemaSchedule.Models.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverIdd { get; set;}
    }
}
