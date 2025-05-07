using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.RequestEntity
{
    public class UserRequestEntity
    {
        public long id { get; set; }
        public string email { get; set; } = null!;

        public string? full_name { get; set; }
        public string? user_name { get; set; }
        public string? contact_no { get; set; }
        public int gender { get; set; }
        public DateOnly? birth_date { get; set; }
        public long? company_id { get; set; }
        public string? role { get; set; }
    }
}
