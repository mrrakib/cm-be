using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
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
using static COLLECTION_MANAGEMENT_UTILITY.CommonEnum;

namespace COLLECTION_MANAGEMENT_SERVICE.Manager
{
    public class EnumDropdownManager : IEnumDropdownManager
    {
        private readonly ICommonManager _commonManager;
        private readonly ILogger _logger;

        public EnumDropdownManager(ICommonManager commonManager, ILogger logger)
        {
            _commonManager = commonManager;
            _logger = logger;
        }

        public async Task<CommonResponse> GetDismantleStatusDDLAsync()
        {
            CommonResponse response = new();
            try
            {
                var companyTypes = Enum.GetValues(typeof(DismantleStatus))
                    .Cast<DismantleStatus>()
                    .Select(e => new { id = (int)e, name = CommonEnum.GetDescription(e) })
                    .ToList();

                response.data = companyTypes;

                return await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching dismantle status types: {WebUtility.HtmlEncode(ex.ToString())}");
                return await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
        }
    }
}
