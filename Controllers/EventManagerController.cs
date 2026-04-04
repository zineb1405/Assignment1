using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewData["Message"] = "Select an event to manage attendees.";

            var events = _context.Events.ToList();
            return View(events);
        }

        // GET: /EventManager/ManageAttendees/1
        [AllowAnonymous]
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
        [Authorize(Roles = "Organizer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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

                TempData["Success"] = "Attendee registered!";
            }

            return RedirectToAction(nameof(ManageAttendees), new { id = id });
        }

        [Authorize(Roles = "Organizer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveAttendee(int id, string attendeeId)
        {
            var attendee = _context.Attendees
                .FirstOrDefault(a => a.Id == attendeeId && a.EventId == id);

            if (attendee == null)
            {
                return NotFound();
            }

            _context.Attendees.Remove(attendee);
            _context.SaveChanges();

            TempData["Success"] = "Attendee removed successfully.";
            return RedirectToAction(nameof(ManageAttendees), new { id = id });
        }
    }
}