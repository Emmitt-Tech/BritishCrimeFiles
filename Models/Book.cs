namespace UKCrimeWeb.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int? YearPublished { get; set; }
        public string? ISBN { get; set; }
        public bool? InPrint { get; set; }
        public decimal? GoodreadsRating { get; set; }
        public string? PurchaseUrl { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<PersonBook> PersonBooks { get; set; } = new List<PersonBook>();
    }
}
