using COLLECTION_MANAGEMENT_API.Attributes;
using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_SERVICE.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using ILogger = Serilog.ILogger;

namespace COLLECTION_MANAGEMENT_API.Controllers
{
    [Route("api/user-role")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleManager _userRoleManager;
        private readonly ILogger _logger;

        public UserRoleController(IUserRoleManager userRoleManager, ILogger logger)
        {
            _userRoleManager = userRoleManager;
            _logger = logger;
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(int page, int pageSize)
        {
            page = page > 1 ? page : 1;
            pageSize = pageSize > 0 ? pageSize : 10;
            _logger.Information($"UserRoleController/GetAll ==> request entity: page: {page}, pageSize: {pageSize}");
            CommonResponse response = await _userRoleManager.GetAllPagedAsync(page, pageSize);
            _logger.Information($"UserRoleController/GetAll ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            _logger.Information($"UserRoleController/Get ==> request entity: user id => {id}");
            CommonResponse response = await _userRoleManager.GetByIdAsync(id);
            _logger.Information($"UserRoleController/Get ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }


        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] UserRoleRequestEntity userRoleRequestEntity)
        {
            _logger.Information($"UserRoleController/Create ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(userRoleRequestEntity, Formatting.None))}");
            CommonResponse response = await _userRoleManager.CreateAsync(userRoleRequestEntity);
            _logger.Information($"UserRoleController/Create ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UserRoleRequestEntity userRoleRequestEntity)
        {
            _logger.Information($"UserRoleController/Update ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(userRoleRequestEntity, Formatting.None))}");
            CommonResponse response = await _userRoleManager.UpdateAsync(userRoleRequestEntity);
            _logger.Information($"UserRoleController/Update ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);

        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(UserRoleRequestEntity userRoleRequestEntity)
        {
            _logger.Information($"UserRoleController/Delete ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(userRoleRequestEntity))}");
            CommonResponse response = await _userRoleManager.DeleteAsync(userRoleRequestEntity);
            _logger.Information($"UserRoleController/Delete ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

    }
}
