using System.ComponentModel.DataAnnotations;

namespace LinkShortener.Models
{
    public class UrlRecord
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Long URL is required.")]
        [Url(ErrorMessage = "Invalid URL format.")]
        public string LongUrl { get; set; }

        public string? ShortUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int ClickCount { get; set; } = 0;
    }
}
