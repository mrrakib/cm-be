using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Interface
{
    public interface IMenuRepository : IBaseRepository<Menu>
    {
        Task<bool> MenuExistsAsync(string menuName, long id = 0);
        Task<Tuple<List<MenuResponseEntity>, int>> GetAllMenus(int page, int pageSize);
        Task<List<MenuResponseEntity>> GetAllMenuByModuleId(long moduleId);
        Task<List<MenuResponseEntity>> GetAllMenuByModuleIdForPermission(GetMenuPermissionRequestEntity menuPermissionRequest);
    }
}
