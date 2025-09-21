using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using COLLECTION_MANAGEMENT_REPOSITORY.Repository;
using COLLECTION_MANAGEMENT_REPOSITORY.UoW;
using COLLECTION_MANAGEMENT_SERVICE.Interface;
using COLLECTION_MANAGEMENT_UTILITY;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_SERVICE.Manager
{
    public interface IMembersBillManager
    {
        Task<CommonResponse> GetAllAsync(int page, int pageSize);
        Task<CommonResponse> CreateAsync(MembersBillRequestEntity collectionTypeRequestEntity);
        Task<CommonResponse> GetByIdAsync(long id);
        Task<CommonResponse> UpdateAsync(MembersBillRequestEntity collectionTypeRequestEntity);
        Task<CommonResponse> DeleteAsync(long id);
    }
    public class MembersBillManager : IMembersBillManager
    {
        public readonly IUnitOfWork _unitOfWork;
        private readonly ICommonManager _commonManager;
        private readonly ILogger _logger;

        public MembersBillManager(IUnitOfWork unitOfWork, ICommonManager commonManager, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _commonManager = commonManager;
            _logger = logger;
        }

        public async Task<CommonResponse> GetAllAsync(int page, int pageSize)
        {
            CommonResponse response = new();
            try
            {
                _logger.Information($"CollectionTypeManager/GetAllAsync ==> request entity: page: {page}, pageSize: {pageSize}");
                Tuple<List<MembersBillResponseEntity>, int> collectionTypes = await _unitOfWork.MembersBill.GetAllPagedAsync(page, pageSize);
                response.data = collectionTypes.Item1;
                response.total_items = collectionTypes.Item2;
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"CollectionTypeManager/GetAllAsync ==> Error fetching collection type: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> CreateAsync(MembersBillRequestEntity membersBillRequestEntity)
        {
            CommonResponse commonResponse = new();
            try
            {
                string? currentUserId = _commonManager.GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, commonResponse);
                }

                string? currentOrganizationId = _commonManager.GetCurrentOrgId();
                if (string.IsNullOrWhiteSpace(currentOrganizationId))
                {
                    _logger.Information($"CollectionTypeManager/CreateAsync ==> organization not found for this user");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.OrganizationNotFound, commonResponse);
                }


                if (await _unitOfWork.Members.GetByIdAsync(membersBillRequestEntity.member_id) == null)
                {
                    _logger.Information($"MemberManager/CreateAsync ==> Member not found using member id: {membersBillRequestEntity.member_id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, commonResponse);
                }


                if (await _unitOfWork.MembersBill.BillExistsAsync(membersBillRequestEntity.member_id, membersBillRequestEntity.from_date, membersBillRequestEntity.to_date))
                {
                    _logger.Information($"CollectionTypeManager/CreateAsync ==> members bill already exists with the given params: {JsonConvert.SerializeObject(membersBillRequestEntity)}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.AlreadyExists, commonResponse);
                }



                MembersBill membersBill = new MembersBill
                {
                    MemberId = membersBillRequestEntity.member_id,
                    FromDate = membersBillRequestEntity.from_date,
                    ToDate = membersBillRequestEntity.to_date,
                    Amount = membersBillRequestEntity.amount,
                    OrgId = long.Parse(currentOrganizationId),
                    CreatedBy = long.Parse(currentUserId),
                };

                await _unitOfWork.MembersBill.AddAsync(membersBill);
                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToCreate, commonResponse);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, commonResponse);
            }
            catch (Exception ex)
            {
                _logger.Error($"CollectionTypeManager/CreateAsync ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, commonResponse);
            }
        }
        public async Task<CommonResponse> GetByIdAsync(long id)
        {
            CommonResponse response = new();
            try
            {
                MembersBill? membersBill = await _unitOfWork.MembersBill.GetByIdAsync(id);
                if (membersBill == null)
                {
                    _logger.Error($"CollectionTypeManager/CreateAsync ==> members bill setup not found with id: {id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }
                _logger.Information($"CollectionTypeManager/CreateAsync ==> member bill type found with id: {id}");

                Member? member = await _unitOfWork.Members.GetByIdAsync(membersBill.MemberId);


                response.data = new MembersBillResponseEntity
                {
                    id = membersBill.Id,
                    from_date = membersBill.FromDate.ToString("dd MMM, yyyy"),
                    to_date = membersBill.ToDate.ToString("dd MMM, yyyy"),
                    member_name = member != null ? member.Name : "N/A",
                    amount = membersBill.Amount,
                };
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"CollectionTypeManager/CreateAsync ==>  Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> UpdateAsync(MembersBillRequestEntity membersBillRequestEntity)
        {
            CommonResponse response = new();
            try
            {
                MembersBill? membersBill = await _unitOfWork.MembersBill.GetByIdAsync(membersBillRequestEntity.id);
                if (membersBill == null)
                {
                    _logger.Information($"MemberManager/CreateAsync ==> Member bill setup not found using id: {membersBillRequestEntity.id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                string? current_user_id = _commonManager.GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(current_user_id))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, response);
                }

                string? currentOrganizationId = _commonManager.GetCurrentOrgId();
                if (string.IsNullOrWhiteSpace(currentOrganizationId))
                {
                    _logger.Information($"CollectionTypeManager/CreateAsync ==> organization not found for this user");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.OrganizationNotFound, response);
                }

                if (await _unitOfWork.Members.GetByIdAsync(membersBillRequestEntity.member_id) == null)
                {
                    _logger.Information($"MemberManager/CreateAsync ==> Member not found using member id: {membersBillRequestEntity.member_id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }



                if (await _unitOfWork.MembersBill.BillExistsAsync(membersBillRequestEntity.member_id, membersBillRequestEntity.from_date, membersBillRequestEntity.to_date, membersBill.Id))
                {
                    _logger.Information($"CollectionTypeManager/CreateAsync ==> members bill already exists with the given params: {JsonConvert.SerializeObject(membersBillRequestEntity)}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.AlreadyExists, response);
                }

                membersBill.Amount = membersBillRequestEntity.amount;
                membersBill.FromDate = membersBillRequestEntity.from_date;
                membersBill.ToDate = membersBillRequestEntity.to_date;
                membersBill.UpdatedBy = long.Parse(current_user_id);
                membersBill.UpdatedAt = DateTime.Now;

                _unitOfWork.MembersBill.Update(membersBill);

                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    _logger.Information($"CollectionTypeManager/UpdateAsync ==> failed to update collection type with id: {membersBillRequestEntity.id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToUpdate, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"CollectionTypeManager/UpdateAsync ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> DeleteAsync(long id)
        {
            CommonResponse response = new();
            try
            {
                var membersBill = await _unitOfWork.MembersBill.GetByIdAsync(id);
                if (membersBill == null)
                {
                    _logger.Information($"CollectionTypeManager/DeleteAsync ==> no collection type found with id: {id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                #region TODO: will check later for dependancy with other entities
                //if (await _unitOfWork.Modules.FindDependancyAsync(moduleId))
                //{
                //    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.StatusCodes.MenuFoundUnderThisModule, response);
                //} 
                #endregion

                _unitOfWork.MembersBill.Delete(membersBill);
                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    _logger.Information($"CollectionTypeManager/DeleteAsync ==> failed to delete collection type with id: {id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToDelete, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"CollectionTypeManager/DeleteAsync ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

    }
}
