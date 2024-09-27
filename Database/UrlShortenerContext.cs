using LinkShortener.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Database
{
    public class UrlShortenerContext : DbContext
    {
        public DbSet<UrlRecord> UrlRecords { get; set; }

        public UrlShortenerContext(DbContextOptions<UrlShortenerContext> options)
            : base(options) { }
    }
}
