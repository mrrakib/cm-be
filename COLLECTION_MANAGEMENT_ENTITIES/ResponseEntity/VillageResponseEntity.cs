using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity
{
    public class VillageResponseEntity
    {
        public long id { get; set; }
        public string vill_name { get; set; } = null!;
        public string? district { get; set; }
        public string? country { get; set; }
    }
}
