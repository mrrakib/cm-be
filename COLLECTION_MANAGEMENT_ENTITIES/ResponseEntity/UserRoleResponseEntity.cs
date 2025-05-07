using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity
{
    public class UserRoleResponseEntity
    {
        public long id { get; set; }
        public long user_id { get; set; }
        public string? user_name { get; set; }
        public long role_id { get; set; }
        public string? role_name { get; set; }
    }
}
