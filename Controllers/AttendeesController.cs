using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Controllers
{
    //Defines the base route for this controller.
    //So all actions in this controller are connected to a specific event.
    //This route means all attendee actions are linked to one event using the eventId.
    //example: /events/1/attendees ; /events/1/attendees/create

    [Route("events/{eventId}/attendees")]
    public class AttendeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        //This constructor receives the database context through dependency injection
        public AttendeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //This means anyone can open this page, even without logging in
        [AllowAnonymous]
        //This action responds to a GET request at the base route
        ///events/1/attendees
        //This action is called when the user opens the attendees page
        [HttpGet("")]
        //This method shows the attendee list for a specific event
        public IActionResult Index(int eventId)
        {
            var ev = _context.Events 
                .Include(e => e.Attendees) 
                .FirstOrDefault(e => e.Id == eventId);//This gets the event with the matching eventId from the URL

            if (ev == null) 
                return NotFound();

            ViewData["EventId"] = eventId;
            ViewData["EventTitle"] = ev.Title;

            return View(ev.Attendees.ToList());
        }

        // Only Organizer can CREATE
        //This action is restricted to users with the Organizer role.
        //Only logged-in users with the Organizer role can access this action.
        [Authorize(Roles = "Organizer")]
        //This opens the form to create a new attendee.
        //Handles GET request for the create page. /events/1/attendees/create
        [HttpGet("create")]
        //This method displays the create attendee form
        public IActionResult Create(int eventId)
        {
            var ev = _context.Events.FirstOrDefault(e => e.Id == eventId);//I first check that the event exists
            if (ev == null) return NotFound();//If the event is not found, I return 404.
            //I pass event information to the page
            ViewData["EventId"] = eventId;
            ViewData["EventTitle"] = ev.Title;
            //I send an empty attendee object to the form, but I already set the EventId
            return View(new Attendee { EventId = eventId });
        }
        //Only organizers can submit creation.
        [Authorize(Roles = "Organizer")]
        //This action runs when the form is submitted
        [HttpPost("create")]
        //This adds security to the form submission.Protects against CSRF attacks.
        [ValidateAntiForgeryToken]
        //This method receives the attendee data entered by the user
        public IActionResult Create(int eventId, Attendee attendee)
        {
            //Checks that the event exists
            var ev = _context.Events.FirstOrDefault(e => e.Id == eventId);
            if (ev == null) return NotFound();//Returns 404 if event is missing.

            attendee.EventId = eventId;//I assign the attendee to the current event.Because the event ID comes from the route, and we want to be sure it is correct

            if (ModelState.IsValid)//If the form data is valid, then I save it.
            {
                _context.Attendees.Add(attendee);//This adds the new attendee to the database context
                _context.SaveChanges();//saves the change to the database
                return RedirectToAction(nameof(Index), new { eventId });//After saving, I return to the attendee list for that event
            }
            //If validation fails, it sends data back to the form
            ViewData["EventId"] = eventId;
            ViewData["EventTitle"] = ev.Title;
            return View(attendee);
        }

        // Only Organizer can EDIT
        //This action opens the edit form for one attendee inside a specific event.
        [Authorize(Roles = "Organizer")]
        [HttpGet("edit/{id}")]
        public IActionResult Edit(int eventId, string id)
        {
            //Finds the attendee with matching attendee ID and event ID
            var attendee = _context.Attendees
                .FirstOrDefault(a => a.Id == id && a.EventId == eventId);

            if (attendee == null)
                return NotFound();//Returns 404 if attendee does not exist

            var ev = _context.Events.FirstOrDefault(e => e.Id == eventId);//Gets the event information
            //Sends event info to the view
            ViewData["EventId"] = eventId;
            ViewData["EventTitle"] = ev?.Title;// why '?': Because if ev is null, it won’t crash

            return View(attendee);//Returns the edit page with current attendee data
        }
        //Handles submitted edit form
        [Authorize(Roles = "Organizer")]
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int eventId, string id, Attendee attendee)
        {
            //Checks that the route ID matches the form model ID.
            if (id != attendee.Id)
                return NotFound();
            //Checks event exists.
            var ev = _context.Events.FirstOrDefault(e => e.Id == eventId);
            if (ev == null) return NotFound();

            attendee.EventId = eventId;//Makes sure the attendee still belongs to the same event.
            //Checks that submitted data is valid
            if (ModelState.IsValid)
            {
                _context.Update(attendee);//This tells Entity Framework that this attendee needs to be updated.
                _context.SaveChanges();//Saves the update in the database
                return RedirectToAction(nameof(Index), new { eventId });//Returns back to the attendee list.
            }
            //If invalid, returns edit form again with existing data
            ViewData["EventId"] = eventId;
            ViewData["EventTitle"] = ev.Title;
            return View(attendee);
        }

        // Only Organizer can DELETE
        //Shows the delete confirmation page
        [Authorize(Roles = "Organizer")]
        [HttpGet("delete/{id}")]
        public IActionResult Delete(int eventId, string id)
        {
            //Finds the correct attendee for that event
            var attendee = _context.Attendees
                .FirstOrDefault(a => a.Id == id && a.EventId == eventId);

            if (attendee == null)//Stops if attendee is missing.
                return NotFound();

            var ev = _context.Events.FirstOrDefault(e => e.Id == eventId);//Gets the event information.
            //Passes event data to the view.
            ViewData["EventId"] = eventId;
            ViewData["EventTitle"] = ev?.Title;
            //Shows delete confirmation page
            return View(attendee);
        }
        //Handles the final deletion after confirmation.
        [Authorize(Roles = "Organizer")]
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int eventId, string id)
        {
            //Finds the attendee again
            var attendee = _context.Attendees
                .FirstOrDefault(a => a.Id == id && a.EventId == eventId);

            if (attendee == null)
                return NotFound();

            _context.Attendees.Remove(attendee);//This tells Entity Framework to delete this attendee
            _context.SaveChanges();

            return RedirectToAction(nameof(Index), new { eventId });//Returns to the attendee list after delete
        }
    }
}