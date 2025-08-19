using COLLECTION_MANAGEMENT_API.Attributes;
using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_SERVICE.Interface;
using COLLECTION_MANAGEMENT_SERVICE.Manager;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using ILogger = Serilog.ILogger;

namespace COLLECTION_MANAGEMENT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationManager _organizationManager;
        private readonly ILogger _logger;

        public OrganizationController(IOrganizationManager organizationManager, ILogger logger)
        {
            _organizationManager = organizationManager;
            _logger = logger;
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(int page, int pageSize)
        {
            page = page > 1 ? page : 1;
            pageSize = pageSize > 0 ? pageSize : 10;
            _logger.Information($"Organization/GetAll ==> request entity: page: {page}, pageSize: {pageSize}");
            CommonResponse response = await _organizationManager.GetAllAsync(page, pageSize);
            _logger.Information($"Organization/GetAll ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            _logger.Information($"Organization/Get ==> request entity: organization id => {id}");
            CommonResponse response = await _organizationManager.GetByIdAsync(id);
            _logger.Information($"Organization/Get ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-ddl")]
        public async Task<IActionResult> GetDDL()
        {
            _logger.Information($"Organization/GetDDL ==> requested");
            CommonResponse response = await _organizationManager.GetForDDL();
            _logger.Information($"Organization/GetDDL ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] OrganizationRequestEntity organizationRequestEntity)
        {
            _logger.Information($"Organization/Create ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(organizationRequestEntity, Formatting.None))}");
            CommonResponse response = await _organizationManager.CreateAsync(organizationRequestEntity);
            _logger.Information($"Organization/Create ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] OrganizationRequestEntity organizationRequestEntity)
        {
            _logger.Information($"Organization/Update ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(organizationRequestEntity, Formatting.None))}");
            CommonResponse response = await _organizationManager.UpdateAsync(organizationRequestEntity);
            _logger.Information($"Organization/Update ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);

        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.Information($"Organization/Delete ==> request entity: Organization id => {id}");
            CommonResponse response = await _organizationManager.DeleteAsync(id);
            _logger.Information($"Organization/Delete ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

    }
}
