using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Controllers
{
    [Route("events/{eventId}/attendees")]
    public class AttendeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /events/{eventId}/attendees
        [HttpGet("")]
        public IActionResult Index(int eventId)
        {
            var ev = _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefault(e => e.Id == eventId);

            if (ev == null)
            {
                return NotFound();
            }

            ViewData["EventId"] = eventId;
            ViewData["EventTitle"] = ev.Title;

            return View(ev.Attendees.ToList());
        }

        // GET: /events/{eventId}/attendees/create
        [HttpGet("create")]
        public IActionResult Create(int eventId)
        {
            var ev = _context.Events.FirstOrDefault(e => e.Id == eventId);

            if (ev == null)
            {
                return NotFound();
            }

            ViewData["EventId"] = eventId;
            ViewData["EventTitle"] = ev.Title;

            var attendee = new Attendee
            {
                EventId = eventId
            };

            return View(attendee);
        }

        // POST: /events/{eventId}/attendees/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int eventId, Attendee attendee)
        {
            var ev = _context.Events.FirstOrDefault(e => e.Id == eventId);

            if (ev == null)
            {
                return NotFound();
            }

            attendee.EventId = eventId;

            if (ModelState.IsValid)
            {
                _context.Attendees.Add(attendee);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index), new { eventId = eventId });
            }

            ViewData["EventId"] = eventId;
            ViewData["EventTitle"] = ev.Title;
            return View(attendee);
        }

        // GET: /events/{eventId}/attendees/edit/{id}
        [HttpGet("edit/{id}")]
        public IActionResult Edit(int eventId, string id)
        {
            var attendee = _context.Attendees
                .FirstOrDefault(a => a.Id == id && a.EventId == eventId);

            if (attendee == null)
            {
                return NotFound();
            }

            var ev = _context.Events.FirstOrDefault(e => e.Id == eventId);

            ViewData["EventId"] = eventId;
            ViewData["EventTitle"] = ev?.Title;

            return View(attendee);
        }

        // POST: /events/{eventId}/attendees/edit/{id}
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int eventId, string id, Attendee attendee)
        {
            if (id != attendee.Id)
            {
                return NotFound();
            }

            var ev = _context.Events.FirstOrDefault(e => e.Id == eventId);

            if (ev == null)
            {
                return NotFound();
            }

            attendee.EventId = eventId;

            if (ModelState.IsValid)
            {
                _context.Update(attendee);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index), new { eventId = eventId });
            }

            ViewData["EventId"] = eventId;
            ViewData["EventTitle"] = ev.Title;
            return View(attendee);
        }

        // GET: /events/{eventId}/attendees/delete/{id}
        [HttpGet("delete/{id}")]
        public IActionResult Delete(int eventId, string id)
        {
            var attendee = _context.Attendees
                .FirstOrDefault(a => a.Id == id && a.EventId == eventId);

            if (attendee == null)
            {
                return NotFound();
            }

            var ev = _context.Events.FirstOrDefault(e => e.Id == eventId);

            ViewData["EventId"] = eventId;
            ViewData["EventTitle"] = ev?.Title;

            return View(attendee);
        }

        // POST: /events/{eventId}/attendees/delete/{id}
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int eventId, string id)
        {
            var attendee = _context.Attendees
                .FirstOrDefault(a => a.Id == id && a.EventId == eventId);

            if (attendee == null)
            {
                return NotFound();
            }

            _context.Attendees.Remove(attendee);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index), new { eventId = eventId });
        }
    }
}