using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Interface
{
    public interface IMenuPermissionRepository : IBaseRepository<MenuPermission>
    {
        IQueryable<MenuPermissionResponseEntity> GetAllPermissions();
        Task<bool> HasMenuPermission(string? role_name, string? client_url, string? menu_url);
    }
}
