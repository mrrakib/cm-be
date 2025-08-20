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
    [Route("api/fy")]
    [ApiController]
    public class FinancialYearController : ControllerBase
    {
        private readonly IFinancialYearManager _financialYearManager;
        private readonly ILogger _logger;

        public FinancialYearController(IFinancialYearManager financialYearManager, ILogger logger)
        {
            _financialYearManager = financialYearManager;
            _logger = logger;
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(int page, int pageSize)
        {
            page = page > 1 ? page : 1;
            pageSize = pageSize > 0 ? pageSize : 10;
            _logger.Information($"FinancialYear/GetAll ==> request entity: page: {page}, pageSize: {pageSize}");
            CommonResponse response = await _financialYearManager.GetAllAsync(page, pageSize);
            _logger.Information($"FinancialYear/GetAll ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            _logger.Information($"FinancialYear/Get ==> request entity: FinancialYear id => {id}");
            CommonResponse response = await _financialYearManager.GetByIdAsync(id);
            _logger.Information($"FinancialYear/Get ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-ddl")]
        public async Task<IActionResult> GetDDL()
        {
            _logger.Information($"FinancialYear/GetDDL ==> requested");
            CommonResponse response = await _financialYearManager.GetForDDL();
            _logger.Information($"FinancialYear/GetDDL ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] FinYearRequestEntity finYearRequestEntity)
        {
            _logger.Information($"FinancialYear/Create ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(finYearRequestEntity, Formatting.None))}");
            CommonResponse response = await _financialYearManager.CreateAsync(finYearRequestEntity);
            _logger.Information($"FinancialYear/Create ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] FinYearRequestEntity finYearRequestEntity)
        {
            _logger.Information($"FinancialYear/Update ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(finYearRequestEntity, Formatting.None))}");
            CommonResponse response = await _financialYearManager.UpdateAsync(finYearRequestEntity);
            _logger.Information($"FinancialYear/Update ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);

        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.Information($"FinancialYear/Delete ==> request entity: FinancialYear id => {id}");
            CommonResponse response = await _financialYearManager.DeleteAsync(id);
            _logger.Information($"FinancialYear/Delete ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

    }
}
