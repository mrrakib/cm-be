using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using COLLECTION_MANAGEMENT_REPOSITORY.UoW;
using COLLECTION_MANAGEMENT_SERVICE.Interface;
using COLLECTION_MANAGEMENT_UTILITY;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_SERVICE.Manager
{
    public class MenuPermissionManager : IMenuPermissionManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonManager _commonManager;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MenuPermissionManager(IUnitOfWork unitOfWork, ICommonManager commonManager, ILogger logger, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _commonManager = commonManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommonResponse> CreateAsync(MenuPermissionRequestEntity menuPermissionRequestEntity)
        {
            CommonResponse commonResponse = new();
            try
            {
                string? currentUserId = GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, commonResponse);
                }
                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Identity);
                IdentityRole<long>? role = await _unitOfWork.Roles.GetByIdAsync(menuPermissionRequestEntity.role_id);
                if (role == null)
                {
                    _logger.Information($"MenuPermissionManager/CreateAsync ==> no role found with id: {menuPermissionRequestEntity.role_id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.RoleNotFound, commonResponse);
                }
                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Default);
                Menu? menu = await _unitOfWork.Menus.GetByIdAsync(menuPermissionRequestEntity.menu_id);
                if (menu == null)
                {
                    _logger.Information($"MenuPermissionManager/CreateAsync ==> no menu found with id: {menuPermissionRequestEntity.role_id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.MenuNotFound, commonResponse);
                }

                MenuPermission menuPermission = new MenuPermission
                {
                    MenuId = menuPermissionRequestEntity.menu_id,
                    RoleId = menuPermissionRequestEntity.role_id,
                    CreatedBy = long.Parse(currentUserId)
                };

                await _unitOfWork.MenuPermissions.AddAsync(menuPermission);
                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToCreate, commonResponse);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, commonResponse);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occurred while creating menu permission. Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, commonResponse);
            }
        }

        public async Task<CommonResponse> CreateBulkAsync(BulkMenuPermissionRequestEntity menuPermissionRequestEntity)
        {
            CommonResponse commonResponse = new();
            try
            {
                string? currentUserId = GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, commonResponse);
                }
                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Identity);
                IdentityRole<long>? role = await _unitOfWork.Roles.GetByIdAsync(menuPermissionRequestEntity.role_id);
                if (role == null)
                {
                    _logger.Information($"MenuPermissionManager/CreateAsync ==> no role found with id: {menuPermissionRequestEntity.role_id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.RoleNotFound, commonResponse);
                }
                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Default);

                foreach (var permission in menuPermissionRequestEntity.permissions.Where(d => d.is_permitted))
                {
                    Menu? menu = await _unitOfWork.Menus.GetByIdAsync(permission.menu_id);
                    if (menu == null)
                    {
                        _logger.Information($"MenuPermissionManager/CreateAsync ==> no menu found with id: {menuPermissionRequestEntity.role_id}");
                        return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.MenuNotFound, commonResponse);
                    }
                }

                List<MenuPermission> menuPermissions = menuPermissionRequestEntity.permissions.Where(d => d.is_permitted).Select(permission => new MenuPermission
                {
                    MenuId = permission.menu_id,
                    RoleId = menuPermissionRequestEntity.role_id,
                    CreatedBy = long.Parse(currentUserId)
                }).ToList();
                IEnumerable<MenuPermission> allPreviousPermissionByRoleId = await _unitOfWork.MenuPermissions.FindAsync(d => d.RoleId == menuPermissionRequestEntity.role_id);

                if(allPreviousPermissionByRoleId.Any())
                    _unitOfWork.MenuPermissions.DeleteBulk(allPreviousPermissionByRoleId);

                await _unitOfWork.MenuPermissions.AddBulkAsync(menuPermissions);
                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToCreate, commonResponse);
                }
                _logger.Information($"MenuPermissionManager/CreateBulkAsync ==> permission created successfylly");
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, commonResponse);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occurred while creating menu permission. Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, commonResponse);
            }
        }

        public async Task<CommonResponse> GetAllAsync()
        {
            CommonResponse response = new();
            try
            {
                var menuPermissions = await _unitOfWork.MenuPermissions.GetAllPermissions().ToListAsync();
                response.data = menuPermissions;
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching menu permissions: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> GetByIdAsync(long id)
        {
            CommonResponse response = new();
            try
            {
                MenuPermission? menuPermission = await _unitOfWork.MenuPermissions.GetByIdAsync(id);
                if (menuPermission == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Identity);
                IdentityRole<long>? role = await _unitOfWork.Roles.GetByIdAsync(menuPermission.RoleId);
                if (role == null)
                {
                    _logger.Information($"MenuPermissionManager/GetByIdAsync ==> no role found with id: {menuPermission.RoleId}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.ModuleNotFound, response);
                }

                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Default);
                Menu? menu = await _unitOfWork.Menus.GetByIdAsync(menuPermission.MenuId);
                if (menu == null)
                {
                    _logger.Information($"MenuPermissionManager/GetByIdAsync ==> no menu found with id: {menuPermission.MenuId}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.ModuleNotFound, response);
                }

                Module? module = await _unitOfWork.Modules.GetByIdAsync(menu.ModuleId);
                if (module == null)
                {
                    _logger.Information($"MenuPermissionManager/GetByIdAsync ==> no module found with id: {menu.ModuleId}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.ModuleNotFound, response);
                }

                response.data = new MenuPermissionResponseEntity
                {
                    id = menuPermission.Id,
                    menu_name = menu.Name,
                    menu_url = menu.Url,
                    module_name = module.Name,
                    role_name = role.Name,
                    role_id = menuPermission.RoleId,
                    menu_id = menuPermission.MenuId
                };
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching single menu permission using id: {id} Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> CheckPermisionByClientURL(string clientURL, string? menu_url = null)
        {
            CommonResponse commonResponse = new();
            try
            {
                string? role_name = GetCurrentRoleName();
                if (string.IsNullOrWhiteSpace(role_name))
                {
                    _logger.Information($"MenuPermissionManager/CheckPermisionByClientURL ==> rol name not found for logged in user");
                    return await _commonManager.HandleResponse(StatusCodes.Status401Unauthorized, (int)CommonEnum.ResponseCodes.Unauthorized, commonResponse);
                }
                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Identity);
                var userRole = await _unitOfWork.Roles.FindAsync(d => !string.IsNullOrWhiteSpace(d.Name) && d.Name.ToLower().Equals(role_name.ToLower()));
                IdentityRole<long>? role = userRole.FirstOrDefault();
                if (role == null)
                {
                    _logger.Information($"MenuPermissionManager/CheckPermisionByClientURL ==> No role found for name: {role_name}");
                    return await _commonManager.HandleResponse(StatusCodes.Status401Unauthorized, (int)CommonEnum.ResponseCodes.Unauthorized, commonResponse);
                }
                //var menuPermission = await _unitOfWork.MenuPermissions.GetByIdAsync(role.Id);
                //if (menuPermission == null)
                //{
                //    _logger.Information($"MenuPermissionManager/CheckPermisionByClientURL ==> user has no permission for this role: {role_name} with provided url: {clientURL}");
                //    return await _commonManager.HandleResponse(StatusCodes.Status401Unauthorized, (int)CommonEnum.StatusCodes.Unauthorized, commonResponse);
                //}
                //var menus = await _unitOfWork.Menus.FindAsync(d => d.Id == menuPermission.MenuId && !string.IsNullOrWhiteSpace(d.ClientUrl) && d.ClientUrl.ToLower().Equals(clientURL.ToLower()));
                //Menu? menu = menus.FirstOrDefault();
                //if (menu == null)
                //{
                //    _logger.Information($"MenuPermissionManager/CheckPermisionByClientURL ==> no menu found for menu id: {menuPermission.MenuId} and client url: {clientURL}");
                //    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.StatusCodes.MenuNotFound, commonResponse);
                //}
                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Default);
                var hasMenuPermission = await _unitOfWork.MenuPermissions.HasMenuPermission(role_name, clientURL, menu_url);
                if (!hasMenuPermission)
                {
                    _logger.Information($"MenuPermissionManager/CheckPermisionByClientURL ==> no permission found for role_name: {role_name} and client url: {clientURL}");
                    return await _commonManager.HandleResponse(StatusCodes.Status401Unauthorized, (int)CommonEnum.ResponseCodes.Unauthorized, commonResponse);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, commonResponse);
            }
            catch (Exception ex)
            {
                _logger.Error($"MenuPermissionManager/CheckPermisionByClientURL ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, commonResponse);
            }
        }

        public async Task<CommonResponse> UpdateAsync(MenuPermissionRequestEntity menuPermissionRequestEntity)
        {
            CommonResponse response = new();
            try
            {
                string? current_user_id = GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(current_user_id))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, response);
                }
                MenuPermission? menuPermission = await _unitOfWork.MenuPermissions.GetByIdAsync(menuPermissionRequestEntity.id);
                if (menuPermission == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Identity);
                IdentityRole<long>? role = await _unitOfWork.Roles.GetByIdAsync(menuPermissionRequestEntity.role_id);
                if (role == null)
                {
                    _logger.Information($"MenuPermissionManager/UpdateAsync ==> no role found with id: {menuPermissionRequestEntity.role_id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.ModuleNotFound, response);
                }
                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Default);
                Menu? menu = await _unitOfWork.Menus.GetByIdAsync(menuPermissionRequestEntity.menu_id);
                if (menu == null)
                {
                    _logger.Information($"MenuPermissionManager/UpdateAsync ==> no menu found with id: {menuPermissionRequestEntity.role_id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.MenuNotFound, response);
                }

                menuPermission.MenuId = menuPermissionRequestEntity.menu_id;
                menuPermission.RoleId = menuPermissionRequestEntity.role_id;
                menuPermission.UpdatedBy = long.Parse(current_user_id);
                menuPermission.UpdatedAt = DateTime.Now;
                _unitOfWork.MenuPermissions.Update(menuPermission);

                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToUpdate, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating menu permission: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> DeleteAsync(long menuPermissionId)
        {
            CommonResponse response = new();
            try
            {
                var mennuPermission = await _unitOfWork.MenuPermissions.GetByIdAsync(menuPermissionId);
                if (mennuPermission == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                _unitOfWork.MenuPermissions.Delete(mennuPermission);
                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToDelete, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting menu permission: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("user_id")?.Value;
        }

        private string? GetCurrentRoleName()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("role_name")?.Value;
        }
    }
}
