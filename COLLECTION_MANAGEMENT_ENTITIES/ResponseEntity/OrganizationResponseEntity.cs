using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity
{
    public class OrganizationResponseEntity
    {
        public long id { get; set; }
        public string? org_name { get; set; }
        public string? mobile_no { get; set; }
        public string? email { get; set; }
        public string? address { get; set; }
        public string? status { get; set; }
    }
}
