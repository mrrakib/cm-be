using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_SERVICE.Interface
{
    public interface IMenuPermissionManager
    {
        Task<CommonResponse> CreateAsync(MenuPermissionRequestEntity menuPermissionRequestEntity);
        Task<CommonResponse> CreateBulkAsync(BulkMenuPermissionRequestEntity menuPermissionRequestEntity);
        Task<CommonResponse> GetAllAsync();
        Task<CommonResponse> GetByIdAsync(long id);
        Task<CommonResponse> CheckPermisionByClientURL(string clientURL, string? menu_url = null);
        Task<CommonResponse> UpdateAsync(MenuPermissionRequestEntity menuPermissionRequestEntity);
        Task<CommonResponse> DeleteAsync(long moduleId);
    }
}
