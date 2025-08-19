using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.RequestEntity
{
    public partial class VillageRequestEntity
    {
        public long id { get; set; }
        public string vill_name { get; set; } = null!;
        public string? district { get; set; }
        public string? country { get; set; }
    }

    public partial class VillageRequestEntity : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(vill_name))
            {
                results.Add(new ValidationResult("vill_name name is required", new[] { "vill_name_empty" }));
            }

            return results;
        }
    }
}
