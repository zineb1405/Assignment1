using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Assignment1.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Events
        public IActionResult Index()
        {
            var events = _context.Events.ToList();
            return View(events);
        }

        // GET: /Events/Details/1
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Event ev, IFormFile bannerFile)
        {
            if (bannerFile != null && bannerFile.Length > 0)
            {
                var fileName = Path.GetFileName(bannerFile.FileName);
                var filePath = Path.Combine("wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    bannerFile.CopyTo(stream);
                }

                ev.BannerUrl = "/images/" + fileName;
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
        public IActionResult Edit(int id)
        {
            var ev = _context.Events.Find(id);

            if (ev == null)
                return NotFound();

            return View(ev);
        }

        // POST: /Events/Edit/1
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
        public IActionResult Delete(int id)
        {
            var ev = _context.Events.FirstOrDefault(e => e.Id == id);

            if (ev == null)
                return NotFound();

            return View(ev);
        }

        // POST: /Events/Delete/1
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