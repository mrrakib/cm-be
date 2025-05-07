using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_REPOSITORY.Interface;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using COLLECTION_MANAGEMENT_REPOSITORY.UoW;
using COLLECTION_MANAGEMENT_SERVICE.Interface;
using COLLECTION_MANAGEMENT_UTILITY;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity.CommonResponse;

namespace COLLECTION_MANAGEMENT_SERVICE.Manager
{
    public class CommonManager : ICommonManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IResponseMessageCacheManager _responseMessageCacheManager;
        private readonly ICommonRepository _commonRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public CommonManager(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IResponseMessageCacheManager responseMessageCacheManager, ICommonRepository commonRepository, IUnitOfWork unitOfWork, ILogger logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _responseMessageCacheManager = responseMessageCacheManager;
            _commonRepository = commonRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CommonResponse> HandleResponse(int httpStatusCodeEnum, int errorCode, CommonResponse responseEntity, string tag = "", string custom_message = "", int? custom_code = null)
        {
            try
            {
                string code = ((int)errorCode).ToString();
                var responseData = (await _responseMessageCacheManager.GetCachedResponseMessage()).FirstOrDefault(f => f.StatusCode?.Trim() == code.Trim());
                //ReverseMapping? reverseMapping = null;
                //if (custom_code != null)
                //{
                //    reverseMapping = await _commonRepository.GetReverseMappingByResponseCode((int)custom_code);
                //}
                //dynamic responseData = null;
                responseEntity.status_code = httpStatusCodeEnum.ToString().Trim();
                if (errorCode != (int)CommonEnum.ResponseCodes.Success)
                {
                    string? message = string.Empty;
                    string language = string.Empty;
                    if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null)
                    {
                        language = !_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("language") ? string.Empty : Convert.ToString(_httpContextAccessor.HttpContext.Request.Headers.FirstOrDefault(a => a.Key == "language").Value);
                    }
                    if (responseData != null)
                    {
                        if (!string.IsNullOrWhiteSpace(custom_message))
                        {
                            message = custom_message;
                        }
                        else
                        {
                            message = !string.IsNullOrWhiteSpace(language)
                       ? language == "bn" ? responseData.MessageBn : responseData.MessageEn
                       : responseData.MessageEn;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(custom_message))
                        {
                            message = custom_message;
                        }
                        else
                        {
                            message = String.Empty;
                        }
                    }
                    ErrorResponseData errorResponseData = new();
                    errorResponseData.error_code = code;
                    errorResponseData.error_message = message ?? String.Empty;
                    //if (reverseMapping == null)
                    //{
                    //    errorResponseData.error_code = code;
                    //    errorResponseData.error_message = message ?? String.Empty;
                    //}
                    //else
                    //{
                    //    errorResponseData.error_code = reverseMapping.ResponseCode.ToString();
                    //    errorResponseData.error_message = reverseMapping.Description;
                    //}
                    responseEntity.error_messages = new List<ErrorResponseData>
                    {
                        errorResponseData
                    };
                }
                else
                {
                    responseEntity.error_messages = new List<ErrorResponseData>();
                }
                //PropertyInfo? property = responseEntity.data.GetType().GetProperty("is_reversal");
                //if (property != null && reverseMapping != null)
                //{
                //    property.SetValue(responseEntity.data, reverseMapping.Reversal, null);
                //}
                //if (custom_code != null)
                //{
                //    ReverseMapping? reverseMapping = await _commonRepository.GetReverseMappingByResponseCode((int)custom_code);
                //    if (reverseMapping != null)
                //    {
                //        responseEntity.is_reversal = reverseMapping.Reversal;
                //    }
                //}
            }
            catch (Exception ex)
            {
                responseEntity.status_code = StatusCodes.Status500InternalServerError.ToString().Trim();
                responseEntity.error_messages = new List<ErrorResponseData>
                    {
                        new ErrorResponseData
                        {
                            error_code = ((int)HttpStatusCode.InternalServerError).ToString(),
                            error_message = ex.Message
                        }
                    };
            }
            return responseEntity;
        }



        private string GenerateRandomDigits(int length)
        {
            Random random = new Random();
            return new string(Enumerable.Range(0, length).Select(_ => (char)('0' + random.Next(10))).ToArray());
        }

        private string GenerateRandomAlphabets(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();
            return new string(Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }
    }
}
