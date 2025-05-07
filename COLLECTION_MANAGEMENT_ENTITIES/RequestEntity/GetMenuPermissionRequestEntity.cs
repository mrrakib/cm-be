using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.RequestEntity
{
    public partial class GetMenuPermissionRequestEntity
    {
        public long role_id { get; set; }
        public long module_id { get; set; }
    }

    public partial class GetMenuPermissionRequestEntity : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            if (role_id == 0)
            {
                validationResults.Add(new ValidationResult ("role is empty", new[] { "role_empty" }));
            }

            if (module_id == 0)
            {
                validationResults.Add(new ValidationResult("module is empty", new[] { "module_empty" }));
            }
            return validationResults;
        }
    }
}
