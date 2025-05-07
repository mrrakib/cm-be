using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using COLLECTION_MANAGEMENT_REPOSITORY.UoW;
using COLLECTION_MANAGEMENT_SERVICE.Interface;
using COLLECTION_MANAGEMENT_UTILITY;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using COLLECTION_MANAGEMENT_REPOSITORY.Repository;

namespace COLLECTION_MANAGEMENT_SERVICE.Manager
{
    public class ModuleManager : IModuleManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonManager _commonManager;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ModuleManager(IUnitOfWork unitOfWork, ICommonManager commonManager, ILogger logger, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _commonManager = commonManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommonResponse> CreateAsync(ModuleRequestEntity moduleRequestEntity)
        {
            CommonResponse commonResponse = new();
            try
            {
                string? currentUserId = GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, commonResponse);
                }

                if (await _unitOfWork.Modules.ModuleExistsAsync(moduleRequestEntity.module_name))
                {
                    _logger.Information($"ModuleManager/CreateAsync ==> module exists with the given name: {moduleRequestEntity.module_name}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.AlreadyExists, commonResponse);
                }

                Module module = new Module
                {
                    Name = moduleRequestEntity.module_name.Trim(),
                    Remarks = moduleRequestEntity.remarks,
                    CreatedBy = long.Parse(currentUserId)
                };

                await _unitOfWork.Modules.AddAsync(module);
                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToCreate, commonResponse);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, commonResponse);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occurred while creating module. Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, commonResponse);
            }
        }

        public async Task<CommonResponse> GetAllAsync()
        {
            CommonResponse response = new();
            try
            {
                var roles = await _unitOfWork.Modules.GetAllAsync();
                response.data = roles.Select(d => new ModuleResponseEntity
                {
                    id = d.Id,
                    module_name = d.Name,
                    remarks = d.Remarks,
                });
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching modules: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> GetByIdAsync(long id)
        {
            CommonResponse response = new();
            try
            {
                Module? module = await _unitOfWork.Modules.GetByIdAsync(id);
                if (module == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                response.data = new ModuleResponseEntity
                {
                    id = module.Id,
                    module_name = module.Name,
                    remarks = module.Remarks,
                };
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching single module using id: {id} Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }


        public async Task<CommonResponse> UpdateAsync(ModuleRequestEntity moduleRequestEntity)
        {
            CommonResponse response = new();
            try
            {
                string? current_user_id = GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(current_user_id))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, response);
                }
                var module = await _unitOfWork.Modules.GetByIdAsync(moduleRequestEntity.id);
                if (module == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                if (await _unitOfWork.Modules.ModuleExistsAsync(moduleRequestEntity.module_name, module.Id))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.AlreadyExists, response);
                }

                module.Name = moduleRequestEntity.module_name;
                module.Remarks = moduleRequestEntity.remarks;
                module.UpdatedBy = long.Parse(current_user_id);
                module.UpdatedAt = DateTime.Now;
                _unitOfWork.Modules.Update(module);

                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToUpdate, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating module: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> DeleteAsync(long moduleId)
        {
            CommonResponse response = new();
            try
            {
                var module = await _unitOfWork.Modules.GetByIdAsync(moduleId);
                if (module == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                if (await _unitOfWork.Modules.FindDependancyAsync(moduleId))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.MenuFoundUnderThisModule, response);
                }

                _unitOfWork.Modules.Delete(module);
                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToDelete, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting module: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("user_id")?.Value;
        }
    }
}
