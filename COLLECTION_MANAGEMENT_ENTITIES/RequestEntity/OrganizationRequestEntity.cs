using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.RequestEntity
{
    public partial class OrganizationRequestEntity
    {
        public long id { get; set; }
        public string org_name { get; set; } = null!;
        public string? mobile_no { get; set; }
        public string? email { get; set; }
        public string? address { get; set; }
    }

    public partial class OrganizationRequestEntity : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(org_name))
            {
                results.Add(new ValidationResult("org_name name is required", new[] { "org_name_empty" }));
            }

            return results;
        }
    }
}
