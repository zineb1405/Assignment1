using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Controllers
{
    public class EventManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /EventManager
        public IActionResult Index()
        {
            ViewData["Message"] = "Select an event to manage attendees.";

            var events = _context.Events.ToList();
            return View(events);
        }

        // GET: /EventManager/ManageAttendees/1
        [HttpGet]
        public IActionResult ManageAttendees(int id)
        {
            var ev = _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefault(e => e.Id == id);

            if (ev == null)
            {
                return NotFound();
            }

            ViewData["EventTitle"] = ev.Title;
            return View(ev);
        }

        // POST: /EventManager/ManageAttendees/1
        [HttpPost]
        public IActionResult ManageAttendees(int id, Attendee attendee)
        {
            var ev = _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefault(e => e.Id == id);

            if (ev == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(attendee.Name) &&
                !string.IsNullOrWhiteSpace(attendee.Email))
            {
                attendee.EventId = ev.Id;
                _context.Attendees.Add(attendee);
                _context.SaveChanges();

                ViewData["Success"] = "Attendee registered!";
            }

            ev = _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefault(e => e.Id == id);

            ViewData["EventTitle"] = ev?.Title;
            return View(ev);
        }
    }
}
