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
    public class VillageController : ControllerBase
    {
        private readonly IVillageManager _villageManager;
        private readonly ILogger _logger;

        public VillageController(IVillageManager villageManager, ILogger logger)
        {
            _villageManager = villageManager;
            _logger = logger;
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(int page, int pageSize)
        {
            page = page > 1 ? page : 1;
            pageSize = pageSize > 0 ? pageSize : 10;
            _logger.Information($"Village/GetAll ==> request entity: page: {page}, pageSize: {pageSize}");
            CommonResponse response = await _villageManager.GetAllAsync(page, pageSize);
            _logger.Information($"Village/GetAll ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            _logger.Information($"Village/Get ==> request entity: Village id => {id}");
            CommonResponse response = await _villageManager.GetByIdAsync(id);
            _logger.Information($"Village/Get ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-ddl")]
        public async Task<IActionResult> GetDDL()
        {
            _logger.Information($"Village/GetDDL ==> request requested.");
            CommonResponse response = await _villageManager.GetForDDL();
            _logger.Information($"Village/GetDDL ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] VillageRequestEntity villageRequestEntity)
        {
            _logger.Information($"Village/Create ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(villageRequestEntity, Formatting.None))}");
            CommonResponse response = await _villageManager.CreateAsync(villageRequestEntity);
            _logger.Information($"Village/Create ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] VillageRequestEntity villageRequestEntity)
        {
            _logger.Information($"Village/Update ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(villageRequestEntity, Formatting.None))}");
            CommonResponse response = await _villageManager.UpdateAsync(villageRequestEntity);
            _logger.Information($"Village/Update ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);

        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.Information($"Village/Delete ==> request entity: Village id => {id}");
            CommonResponse response = await _villageManager.DeleteAsync(id);
            _logger.Information($"Village/Delete ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

    }
}
