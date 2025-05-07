using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.RequestEntity
{
    public partial class MenuPermissionRequestEntity
    {
        public long id { get; set; }
        public long role_id { get; set; }
        public long menu_id { get; set; }
    }

    public partial class MenuPermissionRequestEntity : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            if (role_id == 0)
            {
                validationResults.Add(new ValidationResult("role is empty", new[] { "role_id_empty" }));
            }
            if (menu_id == 0)
            {
                validationResults.Add(new ValidationResult("menu is empty", new[] { "menu_id_empty" }));
            }

            return validationResults;
        }
    }
}
