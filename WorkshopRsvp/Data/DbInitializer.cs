using Microsoft.EntityFrameworkCore;
using WorkshopRsvp.Models;

namespace WorkshopRsvp.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        // Only seed an empty table, so restarting the app does not duplicate records.
        if (await context.Rsvps.AnyAsync())
        {
            return;
        }

        context.Rsvps.AddRange(
            new Rsvp { FullName = "Alice Martin", Email = "alice@example.com", WorkshopTitle = "Routing Basics", NeedsAccommodation = false, RegisteredOn = new DateTime(2026, 10, 1, 9, 30, 0) },
            new Rsvp { FullName = "Bilal Khan", Email = "bilal@example.com", WorkshopTitle = "EF Core Intro", NeedsAccommodation = true, RegisteredOn = new DateTime(2026, 10, 2, 14, 15, 0) },
            new Rsvp { FullName = "Chloe Tremblay", Email = "chloe@example.com", WorkshopTitle = "Razor Essentials", NeedsAccommodation = false, RegisteredOn = new DateTime(2026, 10, 3, 11, 0, 0) });

        await context.SaveChangesAsync();
    }
}
