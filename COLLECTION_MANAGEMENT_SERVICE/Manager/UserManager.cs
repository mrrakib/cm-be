using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using COLLECTION_MANAGEMENT_REPOSITORY.UoW;
using COLLECTION_MANAGEMENT_SERVICE.Interface;
using COLLECTION_MANAGEMENT_UTILITY;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Net;
using static COLLECTION_MANAGEMENT_UTILITY.CommonEnum;

namespace COLLECTION_MANAGEMENT_SERVICE.Manager
{
    public class UserManager : IUserManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonManager _commonManager;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserManager(IUnitOfWork unitOfWork, ICommonManager commonManager, ILogger logger, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _commonManager = commonManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommonResponse> GetAllAsync()
        {
            CommonResponse response = new();
            try
            {
                var users = await _unitOfWork.Users.GetAllAsync();
                response.data = users.Select(d => new UserResponseEntity
                {
                    id = d.Id,
                    full_name = d.FullName,
                    user_name = d.UserName,
                    email = d.Email
                });
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching users: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> GetAllPagedAsync(int page, int pageSize)
        {
            CommonResponse response = new();
            try
            {
                Tuple<List<UserResponseEntity>, int> users = await _unitOfWork.Users.GetAllPagedAsync(page, pageSize);
                response.data = users.Item1;
                response.total_items = users.Item2;
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching users: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }
        public async Task<CommonResponse> GetByIdAsync(long id)
        {
            CommonResponse response = new();
            try
            {
                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Identity);
                ApplicationUser? user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                response.data = new UserResponseEntity
                {
                    id = user.Id,
                    contact_no = user.ContactNo,
                    user_name = user.UserName,
                    email = user.Email,
                    full_name = user.FullName
                };
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching single user using id: {id} Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }


        public async Task<CommonResponse> UpdateAsync(UserRequestEntity userRequestEntity)
        {
            CommonResponse response = new();
            try
            {
                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Identity);
                var user = await _unitOfWork.Users.GetByIdAsync(userRequestEntity.id);
                if (user == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                if (await _unitOfWork.Users.UserExistsAsync(userRequestEntity.email, user.Id))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.AlreadyExists, response);
                }

                if (!Enum.IsDefined(typeof(CommonEnum.Gender), userRequestEntity.gender))
                {
                    _logger.Information($"AccountController/Register ==> gender {userRequestEntity.gender} is not defined.");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UnprocessableEntity, response);
                }
                

                user.Email = userRequestEntity.email;
                user.UserName = userRequestEntity.email;
                user.FullName = userRequestEntity.full_name ?? user.FullName;
                user.ContactNo = userRequestEntity.contact_no ?? user.ContactNo;
                _unitOfWork.Users.Update(user);

                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToUpdate, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"UserManager/UpdateAsync ==> Error updating user: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> DeleteAsync(long id)
        {
            CommonResponse response = new();
            try
            {
                _unitOfWork.SetActiveContext(CommonEnum.ContextName.Identity);
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }
                if (await _unitOfWork.Users.DeleteWithUserRole(id) == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToDelete, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting user: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> GetAllGenderAsync()
        {
            CommonResponse response = new();
            try
            {
                var companyTypes = Enum.GetValues(typeof(Gender))
                    .Cast<Gender>()
                    .Select(e => new { name = e.ToString(), value = (int)e })
                    .ToList();

                response.data = companyTypes;

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"UserManager/GetAllGenderAsync ==> Error fetching genders: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }
    }
}
