namespace UKCrimeWeb.Models
{
    public class Programme
    {
        public int ProgrammeId { get; set; }

        public string? Title { get; set; }

        public int? BroadcastYear { get; set; }

        public int? RuntimeMinutes { get; set; }

        public string? FilePath { get; set; }

        public bool? Subtitles { get; set; }
    }
}