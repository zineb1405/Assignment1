using Microsoft.AspNetCore.Mvc;
using Assignment1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment1.Controllers
{
    public class EventManagerController : Controller
    {
        // Hardcoded "database"
        private static List<Event> _events = new List<Event>
        {
            new Event { Id = 1, Title = "Career Fair", Date = new DateTime(2026, 2, 1), Location = "Gym" },
            new Event { Id = 2, Title = "Tech Talk", Date = new DateTime(2026, 2, 8), Location = "Auditorium" },
            new Event { Id = 3, Title = "Hack Night", Date = new DateTime(2026, 2, 15), Location = "Library" }
        };

        // GET: /EventManager
        public IActionResult Index()
        {
            ViewData["Message"] = "Select an event to manage attendees.";
            return View(_events); // View(model)
        }

        // GET: /EventManager/ManageAttendees/1
        [HttpGet]
        public IActionResult ManageAttendees(int id)
        {
            var ev = _events.FirstOrDefault(e => e.Id == id); // LINQ FirstOrDefault
            if (ev == null) return NotFound();

            ViewData["EventTitle"] = ev.Title; // ViewData
            return View(ev); // View(model)
        }

        // POST: /EventManager/ManageAttendees/1
        [HttpPost]
        public IActionResult ManageAttendees(int id, Attendee attendee)
        {
            var ev = _events.FirstOrDefault(e => e.Id == id);
            if (ev == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(attendee.Name) && !string.IsNullOrWhiteSpace(attendee.Email))
            {
                ev.Attendees.Add(attendee);
                ViewData["Success"] = "Attendee registered!";
            }

            ViewData["EventTitle"] = ev.Title;
            return View(ev);
        }
    }
}
