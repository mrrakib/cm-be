using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity
{
    public class MembersBillResponseEntity
    {
        public long id { get; set; }
        public string? member_name { get; set; }
        public decimal amount { get; set; }
        public string? to_date { get; set; }
        public string? from_date { get; set; }
        public string? status { get; set; }
    }
}
