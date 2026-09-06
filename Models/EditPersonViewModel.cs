using Microsoft.AspNetCore.Http;

namespace UKCrimeWeb.Models
{
    public class EditPersonViewModel
    {
        public int PersonId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Nickname { get; set; }

        public string? ExistingPhotoPath { get; set; }

        public IFormFile? Photo { get; set; }
    }
}