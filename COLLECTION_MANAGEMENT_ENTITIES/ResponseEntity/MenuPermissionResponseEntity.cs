using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity
{
    public class MenuPermissionResponseEntity
    {
        public long id { get; set; }
        public string? module_name { get; set; }
        public long role_id { get; set; }
        public string? role_name { get; set; }
        public long menu_id { get; set; }
        public string? menu_name { get; set; }
        public string? menu_url { get; set; }
        public string? client_url { get; set; }
    }
}
