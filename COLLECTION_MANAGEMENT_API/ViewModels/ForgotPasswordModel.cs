using System.ComponentModel.DataAnnotations;

namespace COLLECTION_MANAGEMENT_API.ViewModels
{
    public partial class ForgotPasswordModel
    {
        public string email { get; set; } = null!;
    }

    public partial class ForgotPasswordModel : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(email))
            {
                validationResults.Add(new ValidationResult("email is empty.", new[] { "email" }));
            }

            return validationResults;
        }
    }
}
