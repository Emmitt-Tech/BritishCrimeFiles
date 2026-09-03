namespace UKCrimeWeb.Models
{
    public class Case
    {
        public int CaseId { get; set; }

        public string? Title { get; set; }

        public string? Summary { get; set; }

        public int? YearStarted { get; set; }

        public int? YearEnded { get; set; }

        public string? VictimCount { get; set; }

        public string? Location { get; set; }

        public string? ImagePath { get; set; }

        public string? SummaryFull { get; set; }

        public string? Sentence { get; set; }
        public string? Status { get; set; }

        public ICollection<TimelineEvent> TimelineEvents { get; set; } = new List<TimelineEvent>();
    }
}