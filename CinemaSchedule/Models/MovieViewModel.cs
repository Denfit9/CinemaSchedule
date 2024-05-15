﻿namespace CinemaSchedule.Models
{
    public class MovieViewModel
    {
        public Guid Id { get; set; }
        public string MovieName { get; set; }
        public string MovieDescription { get; set; }
        public int AgeRestriction { get; set; }
        public string CinemaId { get; set; }
        public int Duration { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public List<string> checksGenres { get; set; }
        public List<string> checksCountries { get; set; }
    }
}