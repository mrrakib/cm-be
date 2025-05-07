using COLLECTION_MANAGEMENT_API.Attributes;
using COLLECTION_MANAGEMENT_ENTITIES;
using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_REPOSITORY.UoW;
using COLLECTION_MANAGEMENT_SERVICE.Interface;
using COLLECTION_MANAGEMENT_SERVICE.Manager;
using COLLECTION_MANAGEMENT_UTILITY;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Numerics;
using System.Text.Json.Serialization;
using ILogger = Serilog.ILogger;

namespace COLLECTION_MANAGEMENT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleManager _roleManager;
        private readonly ILogger _logger;

        public RoleController(IRoleManager roleManager, ILogger logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllRoles()
        {
            CommonResponse response = await _roleManager.GetAllRolesAsync();
            _logger.Information($"Role/GetAllRoles ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetRole(long id)
        {
            _logger.Information($"Role/GetRole ==> request entity: role id => {id}");
            CommonResponse response = await _roleManager.GetByIdAsync(id);
            _logger.Information($"Role/GetRole ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] RoleRequestEntity roleRequestEntity)
        {
            _logger.Information($"Role/CreateRole ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(roleRequestEntity, Formatting.None))}");
            CommonResponse response = await _roleManager.CreateRoleAsync(roleRequestEntity);
            _logger.Information($"Role/CreateRole ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [TypeFilter(typeof(ValidationFilterAttribute), Order = 2)]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleRequestEntity roleRequestEntity)
        {
            _logger.Information($"Role/UpdateRole ==> request entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(roleRequestEntity, Formatting.None))}");
            CommonResponse response = await _roleManager.UpdateRoleAsync(roleRequestEntity);
            _logger.Information($"Role/UpdateRole ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);

        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteRole(long id)
        {
            _logger.Information($"Role/DeleteRole ==> request entity: role id => {id}");
            CommonResponse response = await _roleManager.DeleteRoleAsync(id);
            _logger.Information($"Role/DeleteRole ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

    }
}
