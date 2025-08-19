using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.RequestEntity
{
    public partial class MemberRequestEntity
    {
        public long id { get; set; }
        public string member_name { get; set; } = null!;
        public string? contact_no { get; set; }
        public string? email { get; set; }
        public string? address { get; set; }
        public long village_id { get; set; }
    }

    public partial class MemberRequestEntity : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(member_name))
            {
                results.Add(new ValidationResult("member_name is required", new[] { "member_name_empty" }));
            }

            return results;
        }
    }
}
