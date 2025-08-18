using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.RequestEntity
{
    public partial class FinYearRequestEntity
    {
        public long id { get; set; }
        public string fin_name { get; set; } = null!;
        public DateOnly from_date { get; set; }
        public DateOnly to_date { get; set; }
    }

    public partial class FinYearRequestEntity : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(fin_name))
            {
                results.Add(new ValidationResult("fin_name name is required", new[] { "fin_name_empty" }));
            }

            return results;
        }
    }
}
