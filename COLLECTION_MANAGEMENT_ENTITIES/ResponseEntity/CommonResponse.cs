using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity
{
    public class CommonResponse
    {
        public string? status_code { get; set; }
        public object data { get; set; } = new object();
        public int total_items { get; set; }
        public List<ErrorResponseData> error_messages { get; set; } = new List<ErrorResponseData>();
        public class ErrorResponseData
        {
            public string? error_code { get; set; }
            public string? error_message { get; set; }
        }
    }
}
