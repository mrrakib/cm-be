using System.ComponentModel.DataAnnotations;

namespace COLLECTION_MANAGEMENT_API.ViewModels
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string password { get; set; } = null!;

        public string? full_name { get; set; }

        public string? role { get; set; }
        public string? contact_no { get; set; }
    }
}
