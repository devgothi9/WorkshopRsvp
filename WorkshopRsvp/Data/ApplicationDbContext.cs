using Microsoft.EntityFrameworkCore;
using WorkshopRsvp.Models;

namespace WorkshopRsvp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Rsvp> Rsvps => Set<Rsvp>();
}
