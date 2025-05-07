using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.RequestEntity
{
    public class RoleRequestEntity
    {
        public long id { get; set; }
        public string role_name { get; set; } = null!;
    }
}
