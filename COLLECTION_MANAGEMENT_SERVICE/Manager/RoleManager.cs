using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
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

namespace COLLECTION_MANAGEMENT_SERVICE.Manager
{
    public class RoleManager : IRoleManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonManager _commonManager;
        private readonly ILogger _logger;

        public RoleManager(IUnitOfWork unitOfWork, ICommonManager commonManager, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _commonManager = commonManager;
            _logger = logger;
        }

        public async Task<CommonResponse> CreateRoleAsync(RoleRequestEntity roleRequestEntity)
        {
            CommonResponse commonResponse = new();
            try
            {
                if (await _unitOfWork.Roles.RoleExistsAsync(roleRequestEntity.role_name))
                {
                    _logger.Information($"RoleManager/CreateRoleAsync ==> Role exists with the given name: {roleRequestEntity.role_name}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.AlreadyExists, commonResponse);
                }

                await _unitOfWork.Roles.AddAsync(new IdentityRole<long> { Name = roleRequestEntity.role_name });
                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Identity);
                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToCreate, commonResponse);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, commonResponse);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occurred while creating role. Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, commonResponse);
            }
        }

        public async Task<CommonResponse> GetAllRolesAsync()
        {
            CommonResponse response = new();
            try
            {
                var roles = await _unitOfWork.Roles.GetAllAsync();
                response.data = roles.Select(d => new RoleResponseEntity
                {
                    id = d.Id,
                    role_name = d.Name,
                });
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching roles: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> GetByIdAsync(long id)
        {
            CommonResponse response = new();
            try
            {
                IdentityRole<long>? roles = await _unitOfWork.Roles.GetByIdAsync(id);
                if (roles == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                response.data = new RoleResponseEntity
                {
                    id = roles.Id,
                    role_name = roles.Name,
                };
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching single role using id: {id} Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }


        public async Task<CommonResponse> UpdateRoleAsync(RoleRequestEntity roleRequestEntity)
        {
            CommonResponse response = new();
            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(roleRequestEntity.id);
                if (role == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                if (await _unitOfWork.Roles.RoleExistsAsync(roleRequestEntity.role_name, role.Id))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.AlreadyExists, response);
                }

                role.Name = roleRequestEntity.role_name;
                _unitOfWork.Roles.Update(role);
                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Identity);

                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToUpdate, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating role: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> DeleteRoleAsync(long roleId)
        {
            CommonResponse response = new();
            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
                if (role == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                if (await _unitOfWork.Roles.FindDependancyAsync(roleId))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserFoundForThisRole, response);
                }

                _unitOfWork.Roles.Delete(role);
                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Identity);
                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToDelete, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting role: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }
    }
}
