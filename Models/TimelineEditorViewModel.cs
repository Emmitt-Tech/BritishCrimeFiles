namespace UKCrimeWeb.Models
{
    public class TimelineEditorViewModel
    {
        public Case Case { get; set; } = null!;

        public List<TimelineEvent> Events { get; set; } = new();

        public TimelineEvent NewEvent { get; set; } = new();
    }
}
