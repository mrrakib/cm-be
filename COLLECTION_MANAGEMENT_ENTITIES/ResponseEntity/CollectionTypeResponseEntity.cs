using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity
{
    public class CollectionTypeResponseEntity
    {
        public long id { get; set; }
        public string? type_name { get; set; }
        public string? description { get; set; }
        public bool is_monthly { get; set; }
    }
}
