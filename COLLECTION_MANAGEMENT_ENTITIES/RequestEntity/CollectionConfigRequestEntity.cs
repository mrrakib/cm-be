using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.RequestEntity
{
    public partial class CollectionConfigRequestEntity
    {
        public long id { get; set; }
        public long member_id { get; set; }
        public decimal collection_amount { get; set; }
        public int from_month { get; set; }
        public int from_year { get; set; }
        public int to_month { get; set; }
        public int to_year { get; set; }
    }

    public partial class CollectionConfigRequestEntity : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            return results;
        }
    }
}
