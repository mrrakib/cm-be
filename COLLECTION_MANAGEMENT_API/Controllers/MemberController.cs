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
    public class MemberController : ControllerBase
    {
        private readonly IMemberManager _memberManager;
        private readonly ILogger _logger;

        public MemberController(IMemberManager memberManager, ILogger logger)
        {
            _memberManager = memberManager;
            _logger = logger;
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(int page, int pageSize)
        {
            page = page > 1 ? page : 1;
            pageSize = pageSize > 0 ? pageSize : 10;
            _logger.Information($"Member/GetAll ==> request entity: page: {page}, pageSize: {pageSize}");
            CommonResponse response = await _memberManager.GetAllAsync(page, pageSize);
            _logger.Information($"Member/GetAll ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            _logger.Information($"Member/Get ==> request entity: Member id => {id}");
            CommonResponse response = await _memberManager.GetByIdAsync(id);
            _logger.Information($"Member/Get ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-ddl")]
        public async Task<IActionResult> GetDDL()
        {
            _logger.Information($"Member/GetDDL ==> requested");
            CommonResponse response = await _memberManager.GetForDDL();
            _logger.Information($"Member/GetDDL ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] MemberRequestEntity memberRequestEntity)
        {
            _logger.Information($"Member/Create ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(memberRequestEntity, Formatting.None))}");
            CommonResponse response = await _memberManager.CreateAsync(memberRequestEntity);
            _logger.Information($"Member/Create ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] MemberRequestEntity memberRequestEntity)
        {
            _logger.Information($"Member/Update ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(memberRequestEntity, Formatting.None))}");
            CommonResponse response = await _memberManager.UpdateAsync(memberRequestEntity);
            _logger.Information($"Member/Update ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);

        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.Information($"Member/Delete ==> request entity: Member id => {id}");
            CommonResponse response = await _memberManager.DeleteAsync(id);
            _logger.Information($"Member/Delete ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

    }
}
