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
    public class MenuPermissionRepository : BaseRepository<MenuPermission>, IMenuPermissionRepository
    {
        private readonly am_dbcontext _dbContext;

        public MenuPermissionRepository(am_dbcontext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<MenuPermissionResponseEntity> GetAllPermissions()
        {
            var menuPermissions = from m in _dbContext.Modules
                                  join mn in _dbContext.Menus on m.Id equals mn.ModuleId
                                  join mp in _dbContext.MenuPermissions on mn.Id equals mp.MenuId
                                  join r in _dbContext.Roles on mp.RoleId equals r.Id into rolePermissions
                                  from rp in rolePermissions.DefaultIfEmpty()
                                  select new MenuPermissionResponseEntity
                                  {
                                      id = mp.Id,
                                      menu_name = mn.Name,
                                      menu_url = mn.Url,
                                      module_name = m.Name,
                                      menu_id = m.Id,
                                      role_id = rp.Id,
                                      role_name = rp.Name,
                                  };
            return menuPermissions;
        }

        public async Task<bool> HasMenuPermission(string? role_name, string? client_url, string? menu_url)
        {
            var menuPermissions = from mn in _dbContext.Menus
                                  join mp in _dbContext.MenuPermissions on mn.Id equals mp.MenuId
                                  join r in _dbContext.Roles on mp.RoleId equals r.Id into rolePermissions
                                  from rp in rolePermissions.DefaultIfEmpty()
                                  select new MenuPermissionResponseEntity
                                  {
                                      id = mp.Id,
                                      menu_name = mn.Name,
                                      menu_url = mn.Url,
                                      role_id = rp.Id,
                                      role_name = rp.Name,
                                  };
            if (!string.IsNullOrWhiteSpace(role_name))
            {
                menuPermissions = menuPermissions.Where(d => !string.IsNullOrWhiteSpace(d.role_name) && d.role_name.ToLower().Equals(role_name.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(menu_url))
            {
                menuPermissions = menuPermissions.Where(d => !string.IsNullOrWhiteSpace(d.menu_url) && d.menu_url.ToLower().Equals(menu_url.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(client_url))
            {
                menuPermissions = menuPermissions.Where(d => !string.IsNullOrWhiteSpace(d.client_url) && d.client_url.ToLower().Equals(client_url.ToLower()));
            }
            return await menuPermissions.CountAsync() > 0;
        }
    }
}
