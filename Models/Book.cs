namespace UKCrimeWeb.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string? Author { get; set; }
        public string? PurchaseUrl { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<PersonBook> PersonBooks { get; set; } = new List<PersonBook>();
    }
}