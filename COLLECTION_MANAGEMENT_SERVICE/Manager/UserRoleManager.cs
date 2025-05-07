using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using COLLECTION_MANAGEMENT_REPOSITORY.UoW;
using COLLECTION_MANAGEMENT_SERVICE.Interface;
using COLLECTION_MANAGEMENT_UTILITY;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Net;

namespace COLLECTION_MANAGEMENT_SERVICE.Manager
{
    public class UserRoleManager : IUserRoleManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonManager _commonManager;
        private readonly ILogger _logger;

        public UserRoleManager(IUnitOfWork unitOfWork, ICommonManager commonManager, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _commonManager = commonManager;
            _logger = logger;
        }

        public async Task<CommonResponse> GetAllPagedAsync(int page, int pageSize)
        {
            CommonResponse response = new();
            try
            {
                Tuple<List<UserRoleResponseEntity>, int> users = await _unitOfWork.UserRoles.GetAllPagedAsync(page, pageSize);
                response.data = users.Item1;
                response.total_items = users.Item2;
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching user roles: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }
        public async Task<CommonResponse> GetByIdAsync(long id)
        {
            CommonResponse response = new();
            try
            {
                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Identity);
                UserRoleResponseEntity? userRole = await _unitOfWork.UserRoles.GetUserRoleByUserId(id);
                if (userRole == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                response.data = userRole;
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching single user role using user id: {id} Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }


        public async Task<CommonResponse> CreateAsync(UserRoleRequestEntity userRoleRequestEntity)
        {
            CommonResponse response = new();
            try
            {
                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Identity);

                if (await _unitOfWork.UserRoles.UserRoleExistsAsync(userRoleRequestEntity.user_id))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.AlreadyExists, response);
                }

                ApplicationUser? user = await _unitOfWork.Users.GetByIdAsync(userRoleRequestEntity.user_id);
                if (user == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, response);
                }

                IdentityRole<long>? role = await _unitOfWork.Roles.GetByIdAsync(userRoleRequestEntity.role_id);
                if (role == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.RoleNotFound, response);
                }

                IdentityUserRole<long> userRole = new IdentityUserRole<long>
                {
                    UserId = userRoleRequestEntity.user_id,
                    RoleId = userRoleRequestEntity.role_id,
                };
                await _unitOfWork.UserRoles.AddAsync(userRole);

                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToUpdate, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error creating user role: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> UpdateAsync(UserRoleRequestEntity userRoleRequestEntity)
        {
            CommonResponse response = new();
            try
            {
                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Identity);
                var userRole = await _unitOfWork.UserRoles.GetUserRoleByUserId(userRoleRequestEntity.user_id);
                if (userRole == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                if (await _unitOfWork.UserRoles.UserRoleExistsAsync(userRoleRequestEntity.user_id, userRoleRequestEntity.role_id))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.AlreadyExists, response);
                }

                ApplicationUser? user = await _unitOfWork.Users.GetByIdAsync(userRoleRequestEntity.user_id);
                if (user == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, response);
                }

                IdentityRole<long>? role = await _unitOfWork.Roles.GetByIdAsync(userRoleRequestEntity.role_id);
                if (role == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.RoleNotFound, response);
                }


                await _unitOfWork.UserRoles.UpdateUserRole(userRoleRequestEntity);

                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToUpdate, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating user role: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> DeleteAsync(UserRoleRequestEntity userRoleRequestEntity)
        {
            CommonResponse response = new();
            try
            {
                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Identity);
                var userRole = await _unitOfWork.UserRoles.FindAsync(d => d.UserId == userRoleRequestEntity.user_id && d.RoleId == userRoleRequestEntity.role_id);
                IdentityUserRole<long>? identityUserRole = userRole.FirstOrDefault();
                if (identityUserRole == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }
                _unitOfWork.UserRoles.Delete(identityUserRole);
                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToDelete, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting user role: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }
    }
}
