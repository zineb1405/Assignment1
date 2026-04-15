using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Assignment1.Data;
using Assignment1.Hubs;
using Assignment1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobService _blobService;
        private readonly IHubContext<EventHub> _hubContext;

        public EventsController(
            ApplicationDbContext context,
            BlobService blobService,
            IHubContext<EventHub> hubContext)
        {
            _context = context;
            _blobService = blobService;
            _hubContext = hubContext;
        }

        // GET: /Events
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }

        // GET: /Events/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var ev = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null)
                return NotFound();

            return View(ev);
        }

        // POST: /Events/Register/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "";
            var userName = User.Identity?.Name ?? userEmail;

            var alreadyRegistered = await _context.Attendees
                .AnyAsync(a => a.EventId == id && a.UserId == userId);

            if (alreadyRegistered)
            {
                TempData["Error"] = "You are already registered for this event.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var attendee = new Attendee
            {
                Name = userName,
                Email = userEmail,
                UserId = userId,
                EventId = id
            };

            _context.Attendees.Add(attendee);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group($"event-{id}")
                .SendAsync("ReceiveAttendeeUpdate");

            if (!string.IsNullOrEmpty(ev.OrganizerUserId))
            {
                await _hubContext.Clients.User(ev.OrganizerUserId)
                    .SendAsync("ReceiveOrganizerNotification",
                        $"{userEmail} just registered for your {ev.Title}.");
            }

            TempData["Success"] = "You registered successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: /Events/Unregister/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unregister(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var attendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.EventId == id && a.UserId == userId);

            if (attendee == null)
            {
                TempData["Error"] = "Registration not found.";
                return RedirectToAction(nameof(Details), new { id });
            }

            _context.Attendees.Remove(attendee);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group($"event-{id}")
                .SendAsync("ReceiveAttendeeUpdate");

            TempData["Success"] = "You unregistered successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: /Events/Create
        [Authorize(Roles = "Organizer")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Events/Create
        [Authorize(Roles = "Organizer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event ev, IFormFile? bannerFile)
        {
            if (bannerFile != null && bannerFile.Length > 0)
            {
                ev.BannerUrl = await _blobService.UploadFileAsync(bannerFile);
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ev.OrganizerUserId = currentUserId;

            if (ModelState.IsValid)
            {
                _context.Events.Add(ev);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(ev);
        }

        // GET: /Events/Edit/5
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var ev = await _context.Events.FindAsync(id);
            if (ev == null)
                return NotFound();

            return View(ev);
        }

        // POST: /Events/Edit/5
        [Authorize(Roles = "Organizer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event ev, IFormFile? bannerFile)
        {
            if (id != ev.Id)
                return NotFound();

            var existingEvent = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (existingEvent == null)
                return NotFound();

            if (bannerFile != null && bannerFile.Length > 0)
            {
                ev.BannerUrl = await _blobService.UploadFileAsync(bannerFile);
            }
            else
            {
                ev.BannerUrl = existingEvent.BannerUrl;
            }

            ev.OrganizerUserId = existingEvent.OrganizerUserId;

            if (ModelState.IsValid)
            {
                _context.Update(ev);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(ev);
        }

        // GET: /Events/Delete/5
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var ev = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null)
                return NotFound();

            return View(ev);
        }

        // POST: /Events/Delete/5
        [Authorize(Roles = "Organizer")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null)
                return NotFound();

            if (ev.Attendees.Any())
            {
                _context.Attendees.RemoveRange(ev.Attendees);
            }

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}