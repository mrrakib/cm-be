using COLLECTION_MANAGEMENT_API.Attributes;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_SERVICE.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using ILogger = Serilog.ILogger;

namespace COLLECTION_MANAGEMENT_API.Controllers
{
    [Route("api/common-ddl")]
    [ApiController]
    public class CommonDropdownController : ControllerBase
    {
        private readonly IEnumDropdownManager _enumDropdownManager;
        private readonly ILogger _logger;

        public CommonDropdownController(IEnumDropdownManager enumDropdownManager, ILogger logger)
        {
            _enumDropdownManager = enumDropdownManager;
            _logger = logger;
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("dismantle-status-ddl")]
        public async Task<IActionResult> GetDismantleStatusDDL()
        {
            CommonResponse response = await _enumDropdownManager.GetDismantleStatusDDLAsync();
            _logger.Information($"CompanyController/GetDismantleStatusDDL ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }
    }
}
