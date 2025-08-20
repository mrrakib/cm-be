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
    [Route("api/collection-type")]
    [ApiController]
    public class CollectionTypeController : ControllerBase
    {
        private readonly ICollectionTypeManager _collectionTypeManager;
        private readonly ILogger _logger;

        public CollectionTypeController(ICollectionTypeManager collectionTypeManager, ILogger logger)
        {
            _collectionTypeManager = collectionTypeManager;
            _logger = logger;
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(int page, int pageSize)
        {
            page = page > 1 ? page : 1;
            pageSize = pageSize > 0 ? pageSize : 10;
            _logger.Information($"CollectionType/GetAll ==> request entity: page: {page}, pageSize: {pageSize}");
            CommonResponse response = await _collectionTypeManager.GetAllAsync(page, pageSize);
            _logger.Information($"CollectionType/GetAll ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            _logger.Information($"CollectionType/Get ==> request entity: CollectionType id => {id}");
            CommonResponse response = await _collectionTypeManager.GetByIdAsync(id);
            _logger.Information($"CollectionType/Get ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-ddl")]
        public async Task<IActionResult> GetDDL()
        {
            _logger.Information($"CollectionType/GetDDL ==> requested");
            CommonResponse response = await _collectionTypeManager.GetForDDL();
            _logger.Information($"CollectionType/GetDDL ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CollectionTypeRequestEntity collectionTypeRequestEntity)
        {
            _logger.Information($"CollectionType/Create ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(collectionTypeRequestEntity, Formatting.None))}");
            CommonResponse response = await _collectionTypeManager.CreateAsync(collectionTypeRequestEntity);
            _logger.Information($"CollectionType/Create ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] CollectionTypeRequestEntity collectionTypeRequestEntity)
        {
            _logger.Information($"CollectionType/Update ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(collectionTypeRequestEntity, Formatting.None))}");
            CommonResponse response = await _collectionTypeManager.UpdateAsync(collectionTypeRequestEntity);
            _logger.Information($"CollectionType/Update ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);

        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.Information($"CollectionType/Delete ==> request entity: CollectionType id => {id}");
            CommonResponse response = await _collectionTypeManager.DeleteAsync(id);
            _logger.Information($"CollectionType/Delete ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

    }
}
