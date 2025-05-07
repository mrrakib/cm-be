using COLLECTION_MANAGEMENT_API.Attributes;
using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_SERVICE.Interface;
using COLLECTION_MANAGEMENT_SERVICE.Manager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using ILogger = Serilog.ILogger;

namespace COLLECTION_MANAGEMENT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IUserManager _userManager;

        public UserController(ILogger logger, IUserManager userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [MenuAuthorize("user/get-all")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(int page, int pageSize)
        {
            page = page > 1 ? page : 1;
            pageSize = pageSize > 0 ? pageSize : 10;
            _logger.Information($"UserController/GetAll ==> request entity: page: {page}, pageSize: {pageSize}");
            CommonResponse response = await _userManager.GetAllPagedAsync(page, pageSize);
            _logger.Information($"UserController/GetAll ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-for-ddl")]
        public async Task<IActionResult> GetAllForDropdwon()
        {
            _logger.Information($"UserController/GetAllForDropdwon ==> request started.");
            CommonResponse response = await _userManager.GetAllAsync();
            _logger.Information($"UserController/GetAllForDropdwon ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            _logger.Information($"UserController/Get ==> request entity: company id => {id}");
            CommonResponse response = await _userManager.GetByIdAsync(id);
            _logger.Information($"UserController/Get ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UserRequestEntity userRequestEntity)
        {
            _logger.Information($"UserController/Update ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(userRequestEntity, Formatting.None))}");
            CommonResponse response = await _userManager.UpdateAsync(userRequestEntity);
            _logger.Information($"UserController/Update ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);

        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.Information($"UserController/Delete ==> request entity: menu id => {id}");
            CommonResponse response = await _userManager.DeleteAsync(id);
            _logger.Information($"UserController/Delete ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-all-gender")]
        public async Task<IActionResult> GetAllGender()
        {
            CommonResponse response = await _userManager.GetAllGenderAsync();
            _logger.Information($"CompanyController/GetCompanyTypes ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }
    }
}
