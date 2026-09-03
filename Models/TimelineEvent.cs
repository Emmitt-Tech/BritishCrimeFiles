namespace UKCrimeWeb.Models
{
    public class TimelineEvent
    {
        public int Id { get; set; }

        public int CaseId { get; set; }

        public Case? Case { get; set; }

        public DateTime? EventDate { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? SortOrder { get; set; }
        public string? DisplayDate { get; set; }
    }
}