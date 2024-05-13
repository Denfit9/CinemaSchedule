using CinemaSchedule.Areas.Identity.Data;

namespace CinemaSchedule.Models
{
    public class UsersListVM
    {
        public IQueryable<User> Users {get; set;}

        public int PageSize { get; set;}
        public int CurrentPage { get; set;}
        public int TotalPages { get; set;}
        public string Term { get; set;}
    }
}
