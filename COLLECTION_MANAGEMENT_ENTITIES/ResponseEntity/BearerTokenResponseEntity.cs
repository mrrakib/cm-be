using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity
{
    public class BearerTokenResponseEntity
    {
        public string? token { get; set; }
        public string? expiry_time { get; set; }
    }
}
