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
    public class MenuManager : IMenuManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonManager _commonManager;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MenuManager(IUnitOfWork unitOfWork, ICommonManager commonManager, ILogger logger, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _commonManager = commonManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommonResponse> CreateAsync(MenuRequestEntity menuRequestEntity)
        {
            CommonResponse commonResponse = new();
            try
            {
                string? currentUserId = GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, commonResponse);
                }

                if (await _unitOfWork.Menus.MenuExistsAsync(menuRequestEntity.menu_name))
                {
                    _logger.Information($"MenuManager/CreateAsync ==> menu exists with the given name: {menuRequestEntity.menu_name}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.AlreadyExists, commonResponse);
                }

                Module? module = await _unitOfWork.Modules.GetByIdAsync(menuRequestEntity.module_id);
                if (module == null)
                {
                    _logger.Information($"MenuManager/CreateAsync ==> no module found with id: {menuRequestEntity.module_id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.ModuleNotFound, commonResponse);
                }


                Menu menu = new Menu
                {
                    Name = menuRequestEntity.menu_name.Trim(),
                    Url = menuRequestEntity.menu_url.Trim(),
                    ClientUrl = menuRequestEntity.client_url.Trim(),
                    ModuleId = menuRequestEntity.module_id,
                    CreatedBy = long.Parse(currentUserId)
                };

                await _unitOfWork.Menus.AddAsync(menu);
                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToCreate, commonResponse);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, commonResponse);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occurred while creating menu. Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, commonResponse);
            }
        }

        public async Task<CommonResponse> GetAllAsync(int page, int pageSize)
        {
            CommonResponse response = new();
            try
            {
                Tuple<List<MenuResponseEntity>, int> menus = await _unitOfWork.Menus.GetAllMenus(page, pageSize);
                response.data = menus.Item1;
                response.total_items = menus.Item2;
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching menus: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> GetByIdAsync(long id)
        {
            CommonResponse response = new();
            try
            {
                Menu? menu = await _unitOfWork.Menus.GetByIdAsync(id);
                if (menu == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                Module? module = await _unitOfWork.Modules.GetByIdAsync(menu.ModuleId);
                if (module == null)
                {
                    _logger.Information($"MenuManager/GetByIdAsync ==> no module found with id: {menu.ModuleId}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.ModuleNotFound, response);
                }

                response.data = new MenuResponseEntity
                {
                    menu_id = menu.Id,
                    menu_name = menu.Name,
                    menu_url = menu.Url,
                    client_url = menu.ClientUrl,
                    module_name = module.Name,
                    module_id = module.Id,
                };
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching single menu using id: {id} Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> GetAllByModuleId(long moduleId)
        {
            CommonResponse response = new();
            try
            {
                Module? module = await _unitOfWork.Modules.GetByIdAsync(moduleId);
                if (module == null)
                {
                    _logger.Information($"MenuManager/GetAllByModuleId ==> No module found with given module id: {moduleId}");
                    return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.ModuleNotFound, response);
                }
                var menus = await _unitOfWork.Menus.GetAllMenuByModuleId(moduleId);

                response.data = menus;
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching menus by moduleId: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> GetAllForPermission(GetMenuPermissionRequestEntity getMenuPermissionRequestEntity)
        {
            CommonResponse response = new();
            try
            {
                Module? module = await _unitOfWork.Modules.GetByIdAsync(getMenuPermissionRequestEntity.module_id);
                if (module == null)
                {
                    _logger.Information($"MenuManager/GetAllForPermission ==> No module found with given module id: {getMenuPermissionRequestEntity.module_id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.ModuleNotFound, response);
                }
                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Identity);
                IdentityRole<long>? role = await _unitOfWork.Roles.GetByIdAsync(getMenuPermissionRequestEntity.role_id);
                if (role == null)
                {
                    _logger.Information($"MenuManager/GetAllForPermission ==> No role found with given role id: {getMenuPermissionRequestEntity.role_id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.RoleNotFound, response);
                }

                var menus = await _unitOfWork.Menus.GetAllMenuByModuleIdForPermission(getMenuPermissionRequestEntity);

                response.data = menus;
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"MenuManager/GetAllForPermission ==> Error fetching menus by role and module id: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }


        public async Task<CommonResponse> UpdateAsync(MenuRequestEntity menuRequestEntity)
        {
            CommonResponse response = new();
            try
            {
                string? current_user_id = GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(current_user_id))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, response);
                }
                Menu? menu = await _unitOfWork.Menus.GetByIdAsync(menuRequestEntity.menu_id);
                if (menu == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                if (await _unitOfWork.Menus.MenuExistsAsync(menuRequestEntity.menu_name, menu.Id))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.AlreadyExists, response);
                }

                Module? module = await _unitOfWork.Modules.GetByIdAsync(menuRequestEntity.module_id);
                if (module == null)
                {
                    _logger.Information($"MenuManager/UpdateAsync ==> no module found with id: {menu.ModuleId}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.ModuleNotFound, response);
                }

                menu.Name = menuRequestEntity.menu_name;
                menu.ModuleId = menuRequestEntity.module_id;
                menu.Url = menuRequestEntity.menu_url;
                menu.ClientUrl = menuRequestEntity.client_url;
                menu.UpdatedBy = long.Parse(current_user_id);
                menu.UpdatedAt = DateTime.Now;
                _unitOfWork.Menus.Update(menu);

                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToUpdate, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating menu: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> DeleteAsync(long menuId)
        {
            CommonResponse response = new();
            try
            {
                var menu = await _unitOfWork.Menus.GetByIdAsync(menuId);
                if (menu == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                #region TODO: will check later for dependancy with role permission
                //if (await _unitOfWork.Modules.FindDependancyAsync(moduleId))
                //{
                //    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.StatusCodes.MenuFoundUnderThisModule, response);
                //} 
                #endregion

                _unitOfWork.Menus.Delete(menu);
                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToDelete, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting menu: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("user_id")?.Value;
        }
    }
}
