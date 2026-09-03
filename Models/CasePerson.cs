namespace UKCrimeWeb.Models
{
    public class CasePerson
    {
        public int CaseId { get; set; }
        public int PersonId { get; set; }
        public string? Role { get; set; }
        public Case? Case { get; set; }
        public Person? Person { get; set; }
        public int? SortOrder { get; set; }   
         }
}