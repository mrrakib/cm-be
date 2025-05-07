using System.ComponentModel.DataAnnotations;

namespace COLLECTION_MANAGEMENT_API.ViewModels
{
    public partial class ResetPasswordModel
    {
        public string email { get; set; } = null!;
        public string token { get; set; } = null!;
        public string new_password { get; set; } = null!;
    }

    public partial class ResetPasswordModel : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(email))
            {
                validationResults.Add(new ValidationResult("email is empty.", new[] { "email" }));
            }
            if (string.IsNullOrWhiteSpace(token))
            {
                validationResults.Add(new ValidationResult("token is empty.", new[] { "reset_token_empty" }));
            }
            if (string.IsNullOrWhiteSpace(new_password))
            {
                validationResults.Add(new ValidationResult("new_password is empty", new[] { "new_password_empty" }));
            }

            return validationResults;
        }
    }
}
