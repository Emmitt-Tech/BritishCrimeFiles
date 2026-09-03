namespace UKCrimeWeb.Models
{
    public class Person
    {
        public int PersonId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Nickname { get; set; }

        public int? YearStarted { get; set; }

        public int? YearCaught { get; set; }

        public int? NumberOfVictims { get; set; }

        public string? Biography { get; set; }

        public string? PhotoPath { get; set; }

        public ICollection<PersonProgramme> PersonProgrammes { get; set; } = new List<PersonProgramme>();
        public ICollection<PersonBook> PersonBooks { get; set; } = new List<PersonBook>();
    }
}
