using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Assignment1.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobService _blobService;

        public EventsController(ApplicationDbContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: /Events
        [AllowAnonymous]
        public IActionResult Index()
        {
            var events = _context.Events.ToList();
            return View(events);
        }

        // GET: /Events/Details/1
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var ev = _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefault(e => e.Id == id);

            if (ev == null)
            {
                return NotFound();
            }

            return View(ev);
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

            if (ModelState.IsValid)
            {
                _context.Events.Add(ev);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(ev);
        }

        // GET: /Events/Edit/1
        [Authorize(Roles = "Organizer")]
        public IActionResult Edit(int id)
        {
            var ev = _context.Events.Find(id);

            if (ev == null)
                return NotFound();

            return View(ev);
        }

        // POST: /Events/Edit/1
        [Authorize(Roles = "Organizer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Event ev)
        {
            if (id != ev.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(ev);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(ev);
        }

        // GET: /Events/Delete/1
        [Authorize(Roles = "Organizer")]
        public IActionResult Delete(int id)
        {
            var ev = _context.Events.FirstOrDefault(e => e.Id == id);

            if (ev == null)
                return NotFound();

            return View(ev);
        }

        // POST: /Events/Delete/1
        [Authorize(Roles = "Organizer")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var ev = _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefault(e => e.Id == id);

            if (ev == null)
                return NotFound();

            if (ev.Attendees.Any())
            {
                _context.Attendees.RemoveRange(ev.Attendees);
            }

            _context.Events.Remove(ev);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}