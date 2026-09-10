using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace UKCrimeWeb.Models
{
    public class BookViewModel
    {
        public int BookId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        public int? YearPublished { get; set; }
        public string? ISBN { get; set; }
        public bool? InPrint { get; set; }
        public decimal? GoodreadsRating { get; set; }
        public string? PurchaseUrl { get; set; }
        public string? ImageUrl { get; set; }

        public List<int> SelectedPersonIds { get; set; } = new();
        public List<SelectListItem> People { get; set; } = new();
    }
}
