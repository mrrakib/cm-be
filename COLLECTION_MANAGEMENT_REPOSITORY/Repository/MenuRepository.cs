using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
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
    public class MenuRepository : BaseRepository<Menu>, IMenuRepository
    {
        private readonly am_dbcontext _dbContext;

        public MenuRepository(am_dbcontext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> MenuExistsAsync(string menuName, long id = 0)
        {
            return await _dbContext.Menus.AnyAsync(d => !string.IsNullOrWhiteSpace(d.Name) && (id != 0 ? d.Id != id : true) && d.Name.ToLower().Equals(menuName.Trim().ToLower()));
        }

        public async Task<Tuple<List<MenuResponseEntity>, int>> GetAllMenus(int page, int pageSize)
        {
            var query = from m in _dbContext.Modules
                        join mn in _dbContext.Menus on m.Id equals mn.ModuleId
                        select new MenuResponseEntity
                        {
                            menu_id = mn.Id,
                            menu_name = mn.Name,
                            menu_url = mn.Url,
                            module_name = m.Name,
                            module_id = m.Id
                        };
            int totalCount = await query.CountAsync();
            var menus = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new Tuple<List<MenuResponseEntity>, int>(menus, totalCount);
        }

        public async Task<List<MenuResponseEntity>> GetAllMenuByModuleId(long moduleId)
        {
            var query = from m in _dbContext.Modules
                        join mn in _dbContext.Menus on m.Id equals mn.ModuleId
                        where mn.ModuleId == moduleId
                        select new MenuResponseEntity
                        {
                            menu_id = mn.Id,
                            menu_name = mn.Name,
                            menu_url = mn.Url,
                            module_name = m.Name,
                            module_id = m.Id
                        };

            return await query.ToListAsync();
        }

        public async Task<List<MenuResponseEntity>> GetAllMenuByModuleIdForPermission(GetMenuPermissionRequestEntity menuPermissionRequest)
        {
            var query = from m in _dbContext.Modules
                        join mn in _dbContext.Menus on m.Id equals mn.ModuleId
                        join mp in _dbContext.MenuPermissions.Where(d => d.RoleId == menuPermissionRequest.role_id)
                            on mn.Id equals mp.MenuId into permissions
                        from permission in permissions.DefaultIfEmpty()
                        where mn.ModuleId == menuPermissionRequest.module_id
                        select new MenuResponseEntity
                        {
                            menu_id = mn.Id,
                            menu_name = mn.Name,
                            menu_url = mn.Url,
                            module_name = m.Name,
                            module_id = m.Id,
                            is_permitted = permission != null
                        };


            return await query.ToListAsync();
        }
    }
}
