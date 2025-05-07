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
    public class MenuController : ControllerBase
    {
        private readonly IMenuManager _menuManager;
        private readonly ILogger _logger;

        public MenuController(IMenuManager menuManager, ILogger logger)
        {
            _menuManager = menuManager;
            _logger = logger;
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(int page, int pageSize)
        {
            page = page > 1 ? page : 1;
            pageSize = pageSize > 0 ? pageSize : 10;
            _logger.Information($"Menu/GetAll ==> request entity: page: {page}, pageSize: {pageSize}");
            CommonResponse response = await _menuManager.GetAllAsync(page, pageSize);
            _logger.Information($"Menu/GetAll ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            _logger.Information($"Menu/Get ==> request entity: menu id => {id}");
            CommonResponse response = await _menuManager.GetByIdAsync(id);
            _logger.Information($"Menu/Get ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-by-module-id/{id}")]
        public async Task<IActionResult> GetByModuleId(long id)
        {
            _logger.Information($"Menu/GetByModuleId ==> request entity: module id => {id}");
            CommonResponse response = await _menuManager.GetAllByModuleId(id);
            _logger.Information($"Menu/GetByModuleId ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("get-menus")]
        public async Task<IActionResult> GetMenusForPermission(GetMenuPermissionRequestEntity getMenuPermissionRequestEntity)
        {
            _logger.Information($"Menu/GetMenusForPermission ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(getMenuPermissionRequestEntity))}");
            CommonResponse response = await _menuManager.GetAllForPermission(getMenuPermissionRequestEntity);
            _logger.Information($"Menu/GetMenusForPermission ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] MenuRequestEntity menuRequestEntity)
        {
            _logger.Information($"Menu/Create ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(menuRequestEntity, Formatting.None))}");
            CommonResponse response = await _menuManager.CreateAsync(menuRequestEntity);
            _logger.Information($"Menu/Create ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] MenuRequestEntity mennuRequestEntity)
        {
            _logger.Information($"Menu/Update ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(mennuRequestEntity, Formatting.None))}");
            CommonResponse response = await _menuManager.UpdateAsync(mennuRequestEntity);
            _logger.Information($"Menu/Update ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);

        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.Information($"Menu/Delete ==> request entity: menu id => {id}");
            CommonResponse response = await _menuManager.DeleteAsync(id);
            _logger.Information($"Menu/Delete ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

    }
}
