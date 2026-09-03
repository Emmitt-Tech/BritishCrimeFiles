using System.Collections.Generic;

namespace UKCrimeWeb.Models
{
    public class SearchResultsViewModel
    {
        public List<Case> Cases { get; set; } = new();
        public List<Person> People { get; set; } = new();
        public string? Search { get; set; }
    }
}