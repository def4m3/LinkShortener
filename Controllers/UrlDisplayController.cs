using LinkShortener.Database;
using LinkShortener.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LinkShortener.Controllers
{
    public class UrlDisplayController : Controller
    {
        private readonly UrlShortenerContext _context;

        public UrlDisplayController(UrlShortenerContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var urls = _context.UrlRecords.ToList();
            return View(urls);
        }

        public IActionResult Delete(int id)
        {
            var urlRecord = _context.UrlRecords.FirstOrDefault(x => x.Id == id);
            if (urlRecord != null)
            {
                _context.UrlRecords.Remove(urlRecord);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpGet("{shortUrl}")]
        public IActionResult RedirectToLongUrl(string shortUrl)
        {
            var urlRecord = _context.UrlRecords.FirstOrDefault(x => x.ShortUrl == shortUrl);
            if (urlRecord == null) return NotFound();

            // Увеличение счетчика переходов
            urlRecord.ClickCount++;
            _context.SaveChanges();

            return Redirect(urlRecord.LongUrl);
        }

    }

}
