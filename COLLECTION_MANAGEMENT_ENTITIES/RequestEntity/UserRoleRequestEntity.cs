using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.RequestEntity
{
    public class UserRoleRequestEntity
    {
        public long id { get; set; }
        public long user_id { get; set; }
        public long role_id { get; set; }
    }
}
