using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Models
{
    public class ApplicationUser : IdentityUser<long>
    {
        public string FullName { get; set; } = string.Empty;
        public string? ContactNo { get; set; }
        public long? OrganizationId { get; set; }
    }
}
