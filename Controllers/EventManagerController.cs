using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Controllers
{
    //This is my controller for managing events and attendees
    public class EventManagerController : Controller
    {
        //This field stores the database context so I can read and update data
        private readonly ApplicationDbContext _context;
        //The framework passes the database context into the controller here
        public EventManagerController(ApplicationDbContext context)
        {
            _context = context;//Stores the injected context into _context.
        }

        // GET: /EventManager
        //Allows anyone to access this action, even without login
        [AllowAnonymous]
        public IActionResult Index() //Defines the Index action
        {
            ViewData["Message"] = "Select an event to manage attendees.";//Stores a message in ViewData

            var events = _context.Events.ToList();//Gets all events from the database and converts them into a list
            return View(events);//Returns the view and passes the events list to it.
        }

        // GET: /EventManager/ManageAttendees/1
        [AllowAnonymous]
        //Specifies that this action handles GET requests
        [HttpGet]
        //This method opens one event and shows its attendees.
        public IActionResult ManageAttendees(int id)
        {
            var ev = _context.Events//Starts a query on the Events table.
                .Include(e => e.Attendees)//Loads the related attendees with the event
                .FirstOrDefault(e => e.Id == id);//Finds the first event with the matching ID, or returns null if not found
            //Checks if the event exists. If not, returns a 404 response.
            if (ev == null)
            {
                return NotFound();
            }

            ViewData["EventTitle"] = ev.Title;//Stores the event title in ViewData
            return View(ev);//Returns the view and sends the full event object
        }

        // POST: /EventManager/ManageAttendees/1
        //Only organizers are allowed to submit this form
        [Authorize(Roles = "Organizer")]
        //Specifies that this action handles POST requests.
        [HttpPost]
        //This adds security validation to the form submission. For security
        [ValidateAntiForgeryToken]
        //Defines the POST version of ManageAttendees
        public IActionResult ManageAttendees(int id, Attendee attendee)
        {
            //Finds the event again and includes its attendees
            var ev = _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefault(e => e.Id == id);

            if (ev == null)
            {
                return NotFound();
            }
            //hecks that Name and Email are not empty or just spaces
            if (!string.IsNullOrWhiteSpace(attendee.Name) &&
                !string.IsNullOrWhiteSpace(attendee.Email))
            {
                attendee.EventId = ev.Id;//Connects the attendee to the selected event.Because the attendee must belong to an event.
                _context.Attendees.Add(attendee);//Adds the attendee to the EF Core context.
                _context.SaveChanges();

                TempData["Success"] = "Attendee registered!";//This stores a success message that will appear on the next page load
            }
            //Redirects back to the same attendee management page.
            return RedirectToAction(nameof(ManageAttendees), new { id = id });
        }

        [Authorize(Roles = "Organizer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveAttendee(int id, string attendeeId)
        {
            //inds the attendee whose ID matches attendeeId and belongs to that event.
            var attendee = _context.Attendees
                .FirstOrDefault(a => a.Id == attendeeId && a.EventId == id);

            if (attendee == null)
            {
                return NotFound();
            }

            _context.Attendees.Remove(attendee);
            _context.SaveChanges();
            //This stores a success message for the next request
            TempData["Success"] = "Attendee removed successfully.";
            //Redirects back to the attendee management page for the same event.
            return RedirectToAction(nameof(ManageAttendees), new { id = id });
        }
    }
}