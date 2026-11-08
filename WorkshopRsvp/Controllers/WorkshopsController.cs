using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopRsvp.Data;
using WorkshopRsvp.Models;

namespace WorkshopRsvp.Controllers
{
    public class WorkshopsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkshopsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Workshops people can choose from (used by the form's drop-down).
        public static readonly string[] WorkshopTitles =
        {
            "Routing Basics",
            "EF Core Intro",
            "Razor Essentials"
        };

        // GET: /Workshops  (landing page)
        public async Task<IActionResult> Index()
        {
            ViewData["RegistrationCount"] = await _context.Rsvps.CountAsync();
            return View(WorkshopTitles);
        }

        // GET: /Workshops/RsvpForm
        [HttpGet]
        public IActionResult RsvpForm()
        {
            ViewData["Workshops"] = WorkshopTitles;
            return View(new Rsvp());
        }

        // POST: /Workshops/Confirm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Confirm(Rsvp model)
        {
            if (!ModelState.IsValid)
            {
                // Show the form again with validation messages.
                ViewData["Workshops"] = WorkshopTitles;
                return View("RsvpForm", model);
            }

            model.RegisteredOn = DateTime.Now;
            // Lab 4 keeps form confirmation read-only; the list shows seeded data.

            ViewData["Message"] = $"Thanks for registering, {model.FullName}!";
            return View(model);
        }

        // GET: /Workshops/Registrations
        public async Task<IActionResult> Registrations()
        {
            var registrations = await _context.Rsvps
                .AsNoTracking()
                .OrderBy(r => r.RegisteredOn)
                .ThenBy(r => r.Id)
                .ToListAsync();
            return View(registrations);
        }
    }
}
