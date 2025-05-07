using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.RequestEntity
{
    public partial class BulkMenuPermissionRequestEntity
    {
        public long id { get; set; }
        public long role_id { get; set; }
        public List<PermissionEntity> permissions { get; set; } = new();
        public class PermissionEntity
        {
            public long menu_id { get; set; }
            public string? menu_name { get; set; }
            public bool is_permitted { get; set; }
        }
    }

    public partial class BulkMenuPermissionRequestEntity : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            if (role_id == 0)
            {
                validationResults.Add(new ValidationResult("role is empty", new[] { "role_id_empty" }));
            }
            if (!permissions.Any(d => d.is_permitted))
            {
                validationResults.Add(new ValidationResult("at leaset one permission is required", new[] { "permission_empty" }));
            }
            foreach (var permission in permissions)
            {
                if (permission.menu_id == 0)
                {
                    validationResults.Add(new ValidationResult("menu is empty", new[] { "menu_id_empty" }));
                }
            }
            return validationResults;
        }
    }
}
