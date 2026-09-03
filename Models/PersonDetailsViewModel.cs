namespace UKCrimeWeb.Models
{
    public class PersonDetailsViewModel
    {
        public Person? Person { get; set; }
        public List<Programme> Programmes { get; set; } = new List<Programme>();
        public List<Book> Books { get; set; } = new();
    }
}