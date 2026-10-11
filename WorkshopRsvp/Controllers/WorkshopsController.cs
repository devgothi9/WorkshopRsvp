using Microsoft.AspNetCore.Mvc;
using WorkshopRsvp.Models;

namespace WorkshopRsvp.Controllers
{
    public class WorkshopsController : Controller
    {
        // Workshops people can choose from (used by the form's drop-down).
        public static readonly string[] WorkshopTitles =
        {
            "Routing Basics",
            "EF Core Intro",
            "Razor Essentials"
        };

        // In-memory list of registrations, stored statically in the controller.
        // It starts with a few hardcoded RSVPs, and new ones are added by Confirm (POST).
        // Note: data resets whenever the app restarts (no database in this lab).
        private static readonly List<Rsvp> _registrations = new()
        {
            new Rsvp { FullName = "Alice Martin", Email = "alice@example.com", WorkshopTitle = "Routing Basics", NeedsAccommodation = false, RegisteredOn = new DateTime(2026, 10, 1, 9, 30, 0) },
            new Rsvp { FullName = "Bilal Khan", Email = "bilal@example.com", WorkshopTitle = "EF Core Intro", NeedsAccommodation = true, RegisteredOn = new DateTime(2026, 10, 2, 14, 15, 0) },
            new Rsvp { FullName = "Chloe Tremblay", Email = "chloe@example.com", WorkshopTitle = "Razor Essentials", NeedsAccommodation = false, RegisteredOn = new DateTime(2026, 10, 3, 11, 0, 0) }
        };

        private static readonly object _lock = new();

        // GET: /Workshops  (landing page)
        public IActionResult Index()
        {
            ViewData["RegistrationCount"] = _registrations.Count;
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
            lock (_lock)
            {
                _registrations.Add(model);
            }

            ViewData["Message"] = $"Thanks for registering, {model.FullName}!";
            return View(model);
        }

        // GET: /Workshops/Registrations
        public IActionResult Registrations()
        {
            List<Rsvp> snapshot;
            lock (_lock)
            {
                snapshot = _registrations.OrderBy(r => r.RegisteredOn).ToList();
            }
            return View(snapshot);
        }
    }
}
