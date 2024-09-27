using LinkShortener.Database;
using LinkShortener.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Controllers
{
    public class UrlManagementController : Controller
    {
        private readonly UrlShortenerContext _context;

        public UrlManagementController(UrlShortenerContext context)
        {
            _context = context;
        }

        // GET: UrlManagement/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UrlManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UrlRecord model)
        {
            if (ModelState.IsValid)
            {
                // Генерация короткого URL (добавьте вашу логику здесь)
                model.ShortUrl = GenerateShortUrl();
                model.CreatedAt = DateTime.UtcNow;
                model.ClickCount = 0;

                _context.UrlRecords.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "UrlDisplay");
            }
            return View(model);
        }

        // GET: UrlManagement/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var urlRecord = await _context.UrlRecords.FindAsync(id);
            if (urlRecord == null)
            {
                return NotFound();
            }
            return View(urlRecord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UrlRecord model)
        {
            if (ModelState.IsValid)
            {
                UrlRecord? existingRecord = await _context.UrlRecords.FindAsync(id);

                if (existingRecord == null)
                {
                    return NotFound();
                }

                existingRecord.LongUrl = model.LongUrl;

                _context.Update(existingRecord);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "UrlDisplay");
            }
            return View(model);
        }
        private bool UrlRecordExists(int id)
        {
            return _context.UrlRecords.Any(e => e.Id == id);
        }

        private string GenerateShortUrl()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }
    }

}
