using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UKCrimeWeb.Models
{
    public class CreatePersonViewModel
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Nickname { get; set; }

        public IFormFile? Photo { get; set; }

        public int? CaseId { get; set; }
        public string? CaseTitle { get; set; }
        public List<SelectListItem> Cases { get; set; } = new();
        public string? Role { get; set; }
        public int? SortOrder { get; set; }
    }
}