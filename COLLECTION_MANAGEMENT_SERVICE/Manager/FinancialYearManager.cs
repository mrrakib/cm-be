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
    public interface IFinancialYearManager
    {
        Task<CommonResponse> GetAllAsync(int page, int pageSize);
        Task<CommonResponse> CreateAsync(FinYearRequestEntity finYearRequestEntity);
        Task<CommonResponse> GetByIdAsync(long id);
        Task<CommonResponse> UpdateAsync(FinYearRequestEntity finYearRequestEntity);
        Task<CommonResponse> DeleteAsync(long id);
        Task<CommonResponse> GetForDDL();
    }
    public class FinancialYearManager : IFinancialYearManager
    {
        public readonly IUnitOfWork _unitOfWork;
        private readonly ICommonManager _commonManager;
        private readonly ILogger _logger;

        public FinancialYearManager(IUnitOfWork unitOfWork, ICommonManager commonManager, ILogger logger)
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
                _logger.Information($"FinancialYearManager/GetAllAsync ==> request entity: page: {page}, pageSize: {pageSize}");
                Tuple<List<FinYearResponseEntity>, int> finYears = await _unitOfWork.FinancialYears.GetAllPagedAsync(page, pageSize);
                response.data = finYears.Item1;
                response.total_items = finYears.Item2;
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"FinancialYearManager/GetAllAsync ==> Error fetching orgs: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> CreateAsync(FinYearRequestEntity finYearRequestEntity)
        {
            CommonResponse commonResponse = new();
            try
            {
                string? currentUserId = _commonManager.GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, commonResponse);
                }

                if (await _unitOfWork.FinancialYears.FinYearExistsAsync(finYearRequestEntity.fin_name))
                {
                    _logger.Information($"FinancialYearManager/CreateAsync ==> financial year exists with the given name: {finYearRequestEntity.fin_name}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.AlreadyExists, commonResponse);
                }


                FinancialYear finYearEntity = new FinancialYear
                {
                    Name = finYearRequestEntity.fin_name.Trim(),
                    FromDate = finYearRequestEntity.from_date,
                    ToDate = finYearRequestEntity.to_date,
                    CreatedBy = long.Parse(currentUserId)
                };

                await _unitOfWork.FinancialYears.AddAsync(finYearEntity);
                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToCreate, commonResponse);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, commonResponse);
            }
            catch (Exception ex)
            {
                _logger.Error($"FinancialYearManager/CreateAsync ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, commonResponse);
            }
        }
        public async Task<CommonResponse> GetByIdAsync(long id)
        {
            CommonResponse response = new();
            try
            {
                FinancialYear? finYear = await _unitOfWork.FinancialYears.GetByIdAsync(id);
                if (finYear == null)
                {
                    _logger.Error($"FinancialYearManager/CreateAsync ==> Financial Year not found with id: {id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }
                _logger.Information($"FinancialYearManager/CreateAsync ==> Financial Year found with id: {id}, Name: {finYear.Name}");
                response.data = new FinYearResponseEntity
                {
                    id = finYear.Id,
                    fin_name = finYear.Name,
                    from_date = finYear.FromDate.ToString("dd MMM, yyyy"),
                    to_date = finYear.ToDate.ToString("dd MMM, yyyy"),
                    status = Enum.GetName(typeof(CommonEnum.Status), finYear.Status)
                };
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"FinancialYearManager/CreateAsync ==>  Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> UpdateAsync(FinYearRequestEntity finYearRequestEntity)
        {
            CommonResponse response = new();
            try
            {
                string? current_user_id = _commonManager.GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(current_user_id))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, response);
                }
                FinancialYear? finYear = await _unitOfWork.FinancialYears.GetByIdAsync(finYearRequestEntity.id);
                if (finYear == null)
                {
                    _logger.Information($"FinancialYearManager/UpdateAsync ==> no financial year found with id: {finYearRequestEntity.id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                if (await _unitOfWork.FinancialYears.FinYearExistsAsync(finYearRequestEntity.fin_name, finYear.Id))
                {
                    _logger.Information($"FinancialYearManager/UpdateAsync ==> financial year exists with the given name: {finYearRequestEntity.fin_name}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.AlreadyExists, response);
                }

                finYear.Name = finYearRequestEntity.fin_name;
                finYear.FromDate = finYearRequestEntity.from_date;
                finYear.ToDate = finYearRequestEntity.to_date;
                finYear.UpdatedBy = long.Parse(current_user_id);
                finYear.UpdatedAt = DateTime.Now;
                _unitOfWork.FinancialYears.Update(finYear);

                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    _logger.Information($"FinancialYearManager/UpdateAsync ==> failed to update financial year with id: {finYearRequestEntity.id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToUpdate, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"FinancialYearManager/UpdateAsync ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> DeleteAsync(long id)
        {
            CommonResponse response = new();
            try
            {
                var finYear = await _unitOfWork.FinancialYears.GetByIdAsync(id);
                if (finYear == null)
                {
                    _logger.Information($"FinancialYearManager/DeleteAsync ==> no financial year found with id: {id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                #region TODO: will check later for dependancy with other entities
                //if (await _unitOfWork.Modules.FindDependancyAsync(moduleId))
                //{
                //    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.StatusCodes.MenuFoundUnderThisModule, response);
                //} 
                #endregion

                _unitOfWork.FinancialYears.Delete(finYear);
                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    _logger.Information($"FinancialYearManager/DeleteAsync ==> failed to delete financial year with id: {id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToDelete, response);
                }

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"FinancialYearManager/DeleteAsync ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> GetForDDL()
        {
            CommonResponse response = new();
            try
            {
                List<DropdownResponseEntity> data =  await _unitOfWork.FinancialYears.GetForDDL();
                response.data = data;

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"FinancialYearManager/GetForDDL ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }
    }
}
