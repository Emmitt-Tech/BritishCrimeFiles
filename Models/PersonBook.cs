namespace UKCrimeWeb.Models
{
    public class PersonBook
    {
        public int PersonId { get; set; }
        public int BookId { get; set; }

        public Person Person { get; set; }
        public Book Book { get; set; }
    }
}