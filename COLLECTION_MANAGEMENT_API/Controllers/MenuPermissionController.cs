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
    [Route("api/menu-permission")]
    [ApiController]
    public class MenuPermissionController : ControllerBase
    {
        private readonly IMenuPermissionManager _menuPermissionManager;
        private readonly ILogger _logger;

        public MenuPermissionController(IMenuPermissionManager menuPermissionManager, ILogger logger)
        {
            _menuPermissionManager = menuPermissionManager;
            _logger = logger;
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAl()
        {
            CommonResponse response = await _menuPermissionManager.GetAllAsync();
            _logger.Information($"MenuPermissionController/GetAll ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            _logger.Information($"MenuPermissionController/Get ==> request entity: menu permission id => {id}");
            CommonResponse response = await _menuPermissionManager.GetByIdAsync(id);
            _logger.Information($"MenuPermissionController/Get ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("check-permission")]
        public async Task<IActionResult> CheckPermission(string client_url, string? menu_url = null)
        {
            _logger.Information($"MenuPermissionController/CheckPermission ==> request entity: Client url => {client_url} menu_url: {menu_url}");
            CommonResponse response = await _menuPermissionManager.CheckPermisionByClientURL(client_url, menu_url);
            _logger.Information($"MenuPermissionController/CheckPermission ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] MenuPermissionRequestEntity menuPermissionRequestEntity)
        {
            _logger.Information($"MenuPermissionController/Create ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(menuPermissionRequestEntity, Formatting.None))}");
            CommonResponse response = await _menuPermissionManager.CreateAsync(menuPermissionRequestEntity);
            _logger.Information($"MenuPermissionController/Create ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("create-bulk")]
        public async Task<IActionResult> CreateBulk([FromBody] BulkMenuPermissionRequestEntity menuPermissionRequestEntity)
        {
            _logger.Information($"MenuPermissionController/CreateBulk ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(menuPermissionRequestEntity, Formatting.None))}");
            CommonResponse response = await _menuPermissionManager.CreateBulkAsync(menuPermissionRequestEntity);
            _logger.Information($"MenuPermissionController/CreateBulk ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] MenuPermissionRequestEntity menuPermissionRequestEntity)
        {
            _logger.Information($"MenuPermissionController/Update ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(menuPermissionRequestEntity, Formatting.None))}");
            CommonResponse response = await _menuPermissionManager.UpdateAsync(menuPermissionRequestEntity);
            _logger.Information($"MenuPermissionController/Update ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);

        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.Information($"MenuPermissionController/Delete ==> request entity: menu permission id => {id}");
            CommonResponse response = await _menuPermissionManager.DeleteAsync(id);
            _logger.Information($"MenuPermissionController/Delete ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

    }
}
