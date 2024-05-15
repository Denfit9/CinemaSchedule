using CinemaSchedule.Areas.Identity.Data;
using CinemaSchedule.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CinemaSchedule.Data;

public class DBContext : IdentityDbContext<User>
{
    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Note> Notes { get; set; }

    public DbSet<Cinema> Cinemas { get; set; }

    public DbSet<Hall> Halls { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<GenreConnector> GenreConnectors { get; set; }
    public DbSet<CountryConnector> CountryConnectors { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
