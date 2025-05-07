using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity
{
    public class UserResponseEntity
    {
        public long id { get; set; }
        public long? organization_id { get; set; }
        public string? organization_name { get; set; }
        public string? user_name { get; set; }
        public string? full_name { get; set; }
        public string? role { get; set; }
        public string? email { get; set; }
        public string? contact_no { get; set; }
        public string? birth_date { get; set; }
        public int? gender { get; set; }
        public string? gender_name { get; set; }
    }
}
