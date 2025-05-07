using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity
{
    public class ModuleResponseEntity
    {
        public long id { get; set; }
        public string module_name { get; set; } = null!;
        public string? remarks { get; set; }
    }
}
