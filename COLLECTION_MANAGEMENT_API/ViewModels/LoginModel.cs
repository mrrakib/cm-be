using System.ComponentModel.DataAnnotations;

namespace COLLECTION_MANAGEMENT_API.ViewModels
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string email { get; set; } = null!;

        [Required]
        public string password { get; set; } = null!;
    }
}
