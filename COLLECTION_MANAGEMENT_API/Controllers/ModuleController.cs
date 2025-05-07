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
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleManager _moduleManager;
        private readonly ILogger _logger;

        public ModuleController(IModuleManager moduleManager, ILogger logger)
        {
            _moduleManager = moduleManager;
            _logger = logger;
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAl()
        {
            CommonResponse response = await _moduleManager.GetAllAsync();
            _logger.Information($"Module/GetAll ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            _logger.Information($"Module/Get ==> request entity: module id => {id}");
            CommonResponse response = await _moduleManager.GetByIdAsync(id);
            _logger.Information($"Module/Get ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ModuleRequestEntity moduleRequestEntity)
        {
            _logger.Information($"Module/Create ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(moduleRequestEntity, Formatting.None))}");
            CommonResponse response = await _moduleManager.CreateAsync(moduleRequestEntity);
            _logger.Information($"Module/Create ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] ModuleRequestEntity moduleRequestEntity)
        {
            _logger.Information($"Module/Update ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(moduleRequestEntity, Formatting.None))}");
            CommonResponse response = await _moduleManager.UpdateAsync(moduleRequestEntity);
            _logger.Information($"Module/Update ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);

        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.Information($"Module/Delete ==> request entity: module id => {id}");
            CommonResponse response = await _moduleManager.DeleteAsync(id);
            _logger.Information($"Module/Delete ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

    }
}
