namespace UKCrimeWeb.Models
{
    public class PersonProgramme
    {
        public int PersonId { get; set; }
        public int ProgrammeId { get; set; }

        public Person? Person { get; set; }
        public Programme? Programme { get; set; }
    }
}