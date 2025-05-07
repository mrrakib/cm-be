using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.RequestEntity
{
    public partial class Verify2FARequestEntity
    {
        public string code { get; set; } = null!;
        public string? email { get; set; }
    }

    public partial class Verify2FARequestEntity : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> result = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(code))
            {
                result.Add(new ValidationResult("code can't be empty", new[] { "code_empty" }));
            }

            return result;
        }
    }
}
