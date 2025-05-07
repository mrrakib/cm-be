using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_SERVICE.Interface
{
    public interface IRoleManager
    {
        Task<CommonResponse> GetAllRolesAsync();
        Task<CommonResponse> GetByIdAsync(long id);
        Task<CommonResponse> CreateRoleAsync(RoleRequestEntity roleRequestEntity);
        Task<CommonResponse> UpdateRoleAsync(RoleRequestEntity roleRequestEntity);
        Task<CommonResponse> DeleteRoleAsync(long id);
    }
}
