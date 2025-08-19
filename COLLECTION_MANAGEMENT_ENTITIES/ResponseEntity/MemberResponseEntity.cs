using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity
{
    public class MemberResponseEntity
    {
        public long id { get; set; }
        public string member_name { get; set; } = null!;
        public string? contact_no { get; set; }
        public string? email { get; set; }
        public string? address { get; set; }
        public long? village_id { get; set; }
        public string? village_name { get; set; }
        public string? status { get; set; }
    }
}
