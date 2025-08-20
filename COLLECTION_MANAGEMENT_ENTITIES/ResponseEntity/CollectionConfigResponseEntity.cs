using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity
{
    public class CollectionConfigResponseEntity
    {
        public long id { get; set; }
        public long member_id { get; set; }
        public decimal collection_amount { get; set; }
        public int from_month { get; set; }
        public int from_year { get; set; }
        public int to_month { get; set; }
        public int to_year { get; set; }
        public string? status { get; set; }
    }
}
