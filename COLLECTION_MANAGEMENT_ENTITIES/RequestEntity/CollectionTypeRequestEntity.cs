using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.RequestEntity
{
    public partial class CollectionTypeRequestEntity
    {
        public long id { get; set; }
        public string type_name { get; set; } = null!;
        public string? description { get; set; }
        public bool is_monthly { get; set; }
    }

    public partial class CollectionTypeRequestEntity : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(type_name))
            {
                results.Add(new ValidationResult("type_name name is required", new[] { "type_name_empty" }));
            }

            return results;
        }
    }
}
