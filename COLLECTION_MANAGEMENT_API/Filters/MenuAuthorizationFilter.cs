using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_API.Filters
{
    public class MenuAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly am_dbcontext _context;
        private readonly string _menuPath;

        public MenuAuthorizationFilter(am_dbcontext context, string menuPath)
        {
            _context = context;
            _menuPath = menuPath;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new JsonResult(new CommonResponse
                {
                    status_code = "401",
                    error_messages = new List<CommonResponse.ErrorResponseData>
                    {
                        new CommonResponse.ErrorResponseData
                        {
                            error_code = "8001",
                            error_message = "Unauthorized Access."
                        }
                    }
                });
                return;
            }

            //var roles = user.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            string role_name = user.FindFirst("role_name")?.Value;
            var role = await _context.Roles.FirstOrDefaultAsync(d => d.Name == role_name);
            //var hasAccess = _context.MenuPermissions
            //    .Any(mp => roles.Contains(mp.RoleId.ToString()) && mp.Menu.Url == _menuPath);

            var hasAccess = _context.MenuPermissions
                .Any(mp => mp.RoleId == role.Id && mp.Menu.Url == _menuPath);

            if (!hasAccess)
            {
                //context.Result = new ForbidResult();
                context.Result = new JsonResult(new CommonResponse
                {
                    status_code = "401",
                    error_messages = new List<CommonResponse.ErrorResponseData>
                    {
                        new CommonResponse.ErrorResponseData
                        {
                            error_code = "8001",
                            error_message = "Access to the requested menu is denied."
                        }
                    }
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
        }
    }
}
