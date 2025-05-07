using COLLECTION_MANAGEMENT_REPOSITORY.Interface;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Repository
{
    public class RoleRepository : BaseRepository<IdentityRole<long>>, IRoleRepository
    {
        private readonly identity_dbcontext _dbContext;

        public RoleRepository(identity_dbcontext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> RoleExistsAsync(string roleName, long id = 0)
        {
            return await _dbContext.Roles.AnyAsync(d => !string.IsNullOrWhiteSpace(d.Name) && (id != 0 ? d.Id != id : true) && d.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> FindDependancyAsync(long id)
        {
            return await _dbContext.UserRoles.AnyAsync(d => d.RoleId == id);
        }
    }
}
