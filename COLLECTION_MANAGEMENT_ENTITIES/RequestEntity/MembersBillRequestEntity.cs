using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.RequestEntity
{
    public class MembersBillRequestEntity
    {
        public long id { get; set; }
        public long member_id { get; set; }
        public decimal amount { get; set; }
        public DateTime to_date { get; set; }
        public DateTime from_date { get; set; }
    }
}
