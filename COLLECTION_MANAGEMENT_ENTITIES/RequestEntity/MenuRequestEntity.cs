using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.RequestEntity
{
    public partial class MenuRequestEntity
    {
        public long menu_id { get; set; }
        public string menu_name { get; set; } = null!;
        public string client_url { get; set; } = null!;
        public string menu_url { get; set; } = null!;
        public long module_id { get; set; }
    }

    public partial class MenuRequestEntity : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();

            if (module_id == 0)
            {
                validationResults.Add(new ValidationResult( "module id cannot be empty", new[] { "module_id_empty" } ));
            }

            return validationResults;
        }
    }
}
