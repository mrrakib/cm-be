using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Interface
{
    public interface IRoleRepository : IBaseRepository<IdentityRole<long>>
    {
        Task<bool> RoleExistsAsync(string roleName, long id = 0);
        Task<bool> FindDependancyAsync(long id);
    }
}
