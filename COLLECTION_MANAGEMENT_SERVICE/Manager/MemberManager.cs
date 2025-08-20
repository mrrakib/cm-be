using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using COLLECTION_MANAGEMENT_REPOSITORY.Repository;
using COLLECTION_MANAGEMENT_REPOSITORY.UoW;
using COLLECTION_MANAGEMENT_SERVICE.Interface;
using COLLECTION_MANAGEMENT_UTILITY;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_SERVICE.Manager
{
    public interface IMemberManager
    {
        Task<CommonResponse> GetAllAsync(int page, int pageSize);
        Task<CommonResponse> CreateAsync(MemberRequestEntity memberRequestEntity);
        Task<CommonResponse> GetByIdAsync(long id);
        Task<CommonResponse> UpdateAsync(MemberRequestEntity memberRequestEntity);
        Task<CommonResponse> DeleteAsync(long id);
        Task<CommonResponse> GetForDDL();
    }
    public class MemberManager : IMemberManager
    {
        public readonly IUnitOfWork _unitOfWork;
        private readonly ICommonManager _commonManager;
        private readonly ILogger _logger;
        private readonly IUserManager _userManager;

        public MemberManager(IUnitOfWork unitOfWork, ICommonManager commonManager, ILogger logger, IUserManager userManager)
        {
            _unitOfWork = unitOfWork;
            _commonManager = commonManager;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<CommonResponse> GetAllAsync(int page, int pageSize)
        {
            CommonResponse response = new();
            try
            {
                _logger.Information($"MemberManager/GetAllAsync ==> request entity: page: {page}, pageSize: {pageSize}");
                Tuple<List<MemberResponseEntity>, int> menus = await _unitOfWork.Members.GetAllPagedAsync(page, pageSize);
                response.data = menus.Item1;
                response.total_items = menus.Item2;
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"MemberManager/GetAllAsync ==> Error fetching orgs: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> CreateAsync(MemberRequestEntity memberRequestEntity)
        {
            CommonResponse commonResponse = new();
            try
            {
                string? currentUserId = _commonManager.GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, commonResponse);
                }

                if (await _unitOfWork.Villages.GetByIdAsync(memberRequestEntity.village_id) == null)
                {
                    _logger.Information($"MemberManager/CreateAsync ==> Village not found using village id: {memberRequestEntity.village_id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, commonResponse);
                }

                ApplicationUser? user = await _unitOfWork.Users.GetByIdAsync(long.Parse(currentUserId));

                Member member = new Member
                {
                    Name = memberRequestEntity.member_name.Trim(),
                    ContactNo = memberRequestEntity.contact_no?.Trim(),
                    Email = memberRequestEntity.email?.Trim(),
                    Address = memberRequestEntity.address?.Trim(),
                    VillageId = memberRequestEntity.village_id,
                    OrgId = user?.OrganizationId ?? 0,
                    CreatedBy = long.Parse(currentUserId)
                };

                await _unitOfWork.Members.AddAsync(member);
                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToCreate, commonResponse);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, commonResponse);
            }
            catch (Exception ex)
            {
                _logger.Error($"MemberManager/CreateAsync ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, commonResponse);
            }
        }
        public async Task<CommonResponse> GetByIdAsync(long id)
        {
            CommonResponse response = new();
            try
            {
                Member? member = await _unitOfWork.Members.GetByIdAsync(id);
                if (member == null)
                {
                    _logger.Error($"MemberManager/GetByIdAsync ==> Member not found with id: {id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }
                _logger.Information($"MemberManager/GetByIdAsync ==> Member found with id: {id}, Name: {member.Name}");

                Village? village = await _unitOfWork.Villages.GetByIdAsync(member.VillageId);

                response.data = new MemberResponseEntity
                {
                    id = member.Id,
                    member_name = member.Name,
                    contact_no = member.ContactNo,
                    email = member.Email,
                    address = member.Address,
                    village_id = village?.Id,
                    village_name = village?.Name,
                    status = Enum.GetName(typeof(CommonEnum.Status), member.Status)
                };
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"MemberManager/GetByIdAsync ==>  Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> UpdateAsync(MemberRequestEntity memberRequestEntity)
        {
            CommonResponse response = new();
            try
            {
                string? current_user_id = _commonManager.GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(current_user_id))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, response);
                }
                Member? member = await _unitOfWork.Members.GetByIdAsync(memberRequestEntity.id);
                if (member == null)
                {
                    _logger.Information($"MemberManager/UpdateAsync ==> no Member found with id: {memberRequestEntity.id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                if (await _unitOfWork.Villages.GetByIdAsync(memberRequestEntity.village_id) == null)
                {
                    _logger.Information($"MemberManager/CreateAsync ==> Village not found using Village id: {memberRequestEntity.village_id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                member.Name = memberRequestEntity.member_name;
                member.ContactNo = memberRequestEntity.contact_no;
                member.Email = memberRequestEntity.email;
                member.Address = memberRequestEntity.address;
                member.VillageId = memberRequestEntity.village_id;
                member.UpdatedBy = long.Parse(current_user_id);
                member.UpdatedAt = DateTime.Now;
                _unitOfWork.Members.Update(member);

                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    _logger.Information($"MemberManager/UpdateAsync ==> failed to update Member with id: {memberRequestEntity.id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToUpdate, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"MemberManager/UpdateAsync ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> DeleteAsync(long id)
        {
            CommonResponse response = new();
            try
            {
                var Member = await _unitOfWork.Members.GetByIdAsync(id);
                if (Member == null)
                {
                    _logger.Information($"MemberManager/DeleteAsync ==> no Member found with id: {id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                #region TODO: will check later for dependancy with other entities
                //if (await _unitOfWork.Modules.FindDependancyAsync(moduleId))
                //{
                //    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.StatusCodes.MenuFoundUnderThisModule, response);
                //} 
                #endregion

                _unitOfWork.Members.Delete(Member);
                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    _logger.Information($"MemberManager/DeleteAsync ==> failed to delete Member with id: {id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToDelete, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"MemberManager/DeleteAsync ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> GetForDDL()
        {
            CommonResponse response = new();
            try
            {
                List<DropdownResponseEntity> data =  await _unitOfWork.Members.GetForDDL();
                response.data = data;

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"MemberManager/GetForDDL ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }
    }
}
