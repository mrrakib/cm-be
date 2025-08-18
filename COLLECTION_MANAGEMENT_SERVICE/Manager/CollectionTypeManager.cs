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
    public interface ICollectionTypeManager
    {
        Task<CommonResponse> GetAllAsync(int page, int pageSize);
        Task<CommonResponse> CreateAsync(CollectionTypeRequestEntity collectionTypeRequestEntity);
        Task<CommonResponse> GetByIdAsync(long id);
        Task<CommonResponse> UpdateAsync(CollectionTypeRequestEntity collectionTypeRequestEntity);
        Task<CommonResponse> DeleteAsync(long id);
        Task<CommonResponse> GetForDDL();
    }
    public class CollectionTypeManager : ICollectionTypeManager
    {
        public readonly IUnitOfWork _unitOfWork;
        private readonly ICommonManager _commonManager;
        private readonly ILogger _logger;

        public CollectionTypeManager(IUnitOfWork unitOfWork, ICommonManager commonManager, ILogger logger)
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
                Tuple<List<CollectionTypeResponseEntity>, int> collectionTypes = await _unitOfWork.CollectionTypes.GetAllPagedAsync(page, pageSize);
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

        public async Task<CommonResponse> CreateAsync(CollectionTypeRequestEntity collectionTypeRequestEntity)
        {
            CommonResponse commonResponse = new();
            try
            {
                string? currentUserId = _commonManager.GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, commonResponse);
                }

                if (await _unitOfWork.CollectionTypes.TypeExistsAsync(collectionTypeRequestEntity.type_name))
                {
                    _logger.Information($"CollectionTypeManager/CreateAsync ==> collection type exists with the given name: {collectionTypeRequestEntity.type_name}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.AlreadyExists, commonResponse);
                }


                CollectionType collectionType = new CollectionType
                {
                    Name = collectionTypeRequestEntity.type_name.Trim(),
                    Description = collectionTypeRequestEntity.description,
                    IsMonthly = collectionTypeRequestEntity.is_monthly
                };

                await _unitOfWork.CollectionTypes.AddAsync(collectionType);
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
                CollectionType? collectionType = await _unitOfWork.CollectionTypes.GetByIdAsync(id);
                if (collectionType == null)
                {
                    _logger.Error($"CollectionTypeManager/CreateAsync ==> collection type not found with id: {id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }
                _logger.Information($"CollectionTypeManager/CreateAsync ==> collection type found with id: {id}, Name: {collectionType.Name}");
                response.data = new CollectionTypeResponseEntity
                {
                    id = collectionType.Id,
                    type_name = collectionType.Name,
                    description = collectionType.Description,
                    is_monthly = collectionType.IsMonthly
                };
                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"CollectionTypeManager/CreateAsync ==>  Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }

        public async Task<CommonResponse> UpdateAsync(CollectionTypeRequestEntity collectionTypeRequestEntity)
        {
            CommonResponse response = new();
            try
            {
                string? current_user_id = _commonManager.GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(current_user_id))
                {
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UserNotFound, response);
                }
                CollectionType? collectionType = await _unitOfWork.CollectionTypes.GetByIdAsync(collectionTypeRequestEntity.id);
                if (collectionType == null)
                {
                    _logger.Information($"CollectionTypeManager/UpdateAsync ==> no collection type found with id: {collectionTypeRequestEntity.id}");
                    return await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response);
                }

                if (await _unitOfWork.CollectionTypes.TypeExistsAsync(collectionTypeRequestEntity.type_name, collectionType.Id))
                {
                    _logger.Information($"CollectionTypeManager/UpdateAsync ==> collection type exists with the given name: {collectionTypeRequestEntity.type_name}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.AlreadyExists, response);
                }

                collectionType.Name = collectionTypeRequestEntity.type_name;
                collectionType.Description = collectionTypeRequestEntity.description;
                collectionType.IsMonthly = collectionTypeRequestEntity.is_monthly;
                _unitOfWork.CollectionTypes.Update(collectionType);

                if (await _unitOfWork.CompleteAsync() == 0)
                {
                    _logger.Information($"CollectionTypeManager/UpdateAsync ==> failed to update collection type with id: {collectionTypeRequestEntity.id}");
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
                var collectionTypes = await _unitOfWork.CollectionTypes.GetByIdAsync(id);
                if (collectionTypes == null)
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

                _unitOfWork.CollectionTypes.Delete(collectionTypes);
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

        public async Task<CommonResponse> GetForDDL()
        {
            CommonResponse response = new();
            try
            {
                List<DropdownResponseEntity> data =  await _unitOfWork.CollectionTypes.GetForDDL();
                response.data = data;

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"CollectionTypeManager/GetForDDL ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }
    }
}
