namespace UKCrimeWeb.Models
{
    public class CaseDetailsViewModel
    {
        public Case? Case { get; set; }

        public List<Person> People { get; set; } = new List<Person>();

        public List<CasePerson> CasePeople { get; set; } = new List<CasePerson>();

        public List<Book> Books { get; set; } = new();

        public List<TimelineEvent> TimelineEvents { get; set; } = new();
    }
}