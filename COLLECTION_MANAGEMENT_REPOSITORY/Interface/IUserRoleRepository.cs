using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Interface
{
    public interface IUserRoleRepository : IBaseRepository<IdentityUserRole<long>>
    {
        Task<Tuple<List<UserRoleResponseEntity>, int>> GetAllPagedAsync(int pageNumber, int pageSize);
        Task<UserRoleResponseEntity?> GetUserRoleByUserId(long userId);
        Task UpdateUserRole(UserRoleRequestEntity userRoleRequestEntity);
        Task<bool> UserRoleExistsAsync(long user_id, long role_id = 0);
    }
}
