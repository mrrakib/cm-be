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
    public interface IOrganizationManager
    {
        Task<CommonResponse> GetAllAsync(int page, int pageSize);
        Task<CommonResponse> CreateAsync(OrganizationRequestEntity organizationRequestEntity);
        Task<CommonResponse> GetByIdAsync(long id);
        Task<CommonResponse> UpdateAsync(OrganizationRequestEntity organizationRequestEntity);
        Task<CommonResponse> DeleteAsync(long id);
        Task<CommonResponse> GetForDDL();
    }
    public class OrganizationManager : IOrganizationManager
    {
        public readonly IUnitOfWork _unitOfWork;
        private readonly ICommonManager _commonManager;
        private readonly ILogger _logger;

        public OrganizationManager(IUnitOfWork unitOfWork, ICommonManager commonManager, ILogger logger)
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
                _logger.Information($"OrganizationManager/GetAllAsync ==> request entity: page: {page}, pageSize: {pageSize}");
                Tuple<List<OrganizationResponseEntity>, int> menus = await _unitOfWork.Organizations.GetAllPagedAsync(page, pageSize);
                response.data = menus.Item1;
                response.total_items = menus.Item2;
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"OrganizationManager/GetAllAsync ==> Error fetching orgs: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> CreateAsync(OrganizationRequestEntity organizationRequestEntity)
        {
            CommonResponse commonResponse = new();
            try
            {
                string? currentUserId = _commonManager.GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, commonResponse);
                }

                if (await _unitOfWork.Organizations.OrgExistsAsync(organizationRequestEntity.org_name))
                {
                    _logger.Information($"OrganizationManager/CreateAsync ==> organization exists with the given name: {organizationRequestEntity.org_name}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.AlreadyExists, commonResponse);
                }


                Organization organization = new Organization
                {
                    Name = organizationRequestEntity.org_name.Trim(),
                    MobileNo = organizationRequestEntity.mobile_no?.Trim(),
                    Email = organizationRequestEntity.email?.Trim(),
                    Address = organizationRequestEntity.address?.Trim(),
                    CreatedBy = long.Parse(currentUserId)
                };

                await _unitOfWork.Organizations.AddAsync(organization);
                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToCreate, commonResponse);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, commonResponse);
            }
            catch (Exception ex)
            {
                _logger.Error($"OrganizationManager/CreateAsync ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, commonResponse);
            }
        }
        public async Task<CommonResponse> GetByIdAsync(long id)
        {
            CommonResponse response = new();
            try
            {
                Organization? org = await _unitOfWork.Organizations.GetByIdAsync(id);
                if (org == null)
                {
                    _logger.Error($"OrganizationManager/CreateAsync ==> Organization not found with id: {id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }
                _logger.Information($"OrganizationManager/CreateAsync ==> Organization found with id: {id}, Name: {org.Name}");
                response.data = new OrganizationResponseEntity
                {
                    id = org.Id,
                    org_name = org.Name,
                    mobile_no = org.MobileNo,
                    email = org.Email,
                    address = org.Address,
                    status = Enum.GetName(typeof(CommonEnum.Status), org.Status)
                };
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"OrganizationManager/CreateAsync ==>  Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> UpdateAsync(OrganizationRequestEntity organizationRequestEntity)
        {
            CommonResponse response = new();
            try
            {
                string? current_user_id = _commonManager.GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(current_user_id))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, response);
                }
                Organization? organization = await _unitOfWork.Organizations.GetByIdAsync(organizationRequestEntity.id);
                if (organization == null)
                {
                    _logger.Information($"OrganizationManager/UpdateAsync ==> no organization found with id: {organizationRequestEntity.id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                if (await _unitOfWork.Organizations.OrgExistsAsync(organizationRequestEntity.org_name, organization.Id))
                {
                    _logger.Information($"OrganizationManager/UpdateAsync ==> organization exists with the given name: {organizationRequestEntity.org_name}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.AlreadyExists, response);
                }

                organization.Name = organizationRequestEntity.org_name;
                organization.MobileNo = organizationRequestEntity.mobile_no;
                organization.Email = organizationRequestEntity.email;
                organization.Address = organizationRequestEntity.address;
                organization.UpdatedBy = long.Parse(current_user_id);
                organization.UpdatedAt = DateTime.Now;
                _unitOfWork.Organizations.Update(organization);

                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    _logger.Information($"OrganizationManager/UpdateAsync ==> failed to update organization with id: {organizationRequestEntity.id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToUpdate, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"OrganizationManager/UpdateAsync ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> DeleteAsync(long id)
        {
            CommonResponse response = new();
            try
            {
                var organization = await _unitOfWork.Organizations.GetByIdAsync(id);
                if (organization == null)
                {
                    _logger.Information($"OrganizationManager/DeleteAsync ==> no organization found with id: {id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                #region TODO: will check later for dependancy with other entities
                //if (await _unitOfWork.Modules.FindDependancyAsync(moduleId))
                //{
                //    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.StatusCodes.MenuFoundUnderThisModule, response);
                //} 
                #endregion

                _unitOfWork.Organizations.Delete(organization);
                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    _logger.Information($"OrganizationManager/DeleteAsync ==> failed to delete organization with id: {id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToDelete, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"OrganizationManager/DeleteAsync ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> GetForDDL()
        {
            CommonResponse response = new();
            try
            {
                var organization = await _unitOfWork.Organizations.GetAllAsync();
                if (organization == null || !organization.Any())
                {
                    _logger.Information($"OrganizationManager/GetForDDL ==> no organization found");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                List<DropdownResponseEntity> data =  await _unitOfWork.Organizations.GetForDDL();
                response.data = data;

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"OrganizationManager/GetForDDL ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }
    }
}
