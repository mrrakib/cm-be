using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_SERVICE.Interface
{
    public interface IMenuManager
    {
        Task<CommonResponse> CreateAsync(MenuRequestEntity menuRequestEntity);
        Task<CommonResponse> GetAllAsync(int page, int pageSize);
        Task<CommonResponse> GetByIdAsync(long id);
        Task<CommonResponse> GetAllByModuleId(long moduleId);
        Task<CommonResponse> GetAllForPermission(GetMenuPermissionRequestEntity getMenuPermissionRequestEntity);
        Task<CommonResponse> UpdateAsync(MenuRequestEntity menuRequestEntity);
        Task<CommonResponse> DeleteAsync(long moduleId);
    }
}
