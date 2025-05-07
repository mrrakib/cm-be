using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace COLLECTION_MANAGEMENT_API.Attributes
{
    public class AuthorizationFilterAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            CommonResponse response = new();

            var user = context.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                response.data = new { message = "Unauthorized access" };
                response.status_code = StatusCodes.Status401Unauthorized.ToString();
                context.Result = new UnauthorizedObjectResult(response);
                return;
            }

            var expClaim = user.FindFirst(JwtRegisteredClaimNames.Exp);
            if (expClaim != null && long.TryParse(expClaim.Value, out long exp))
            {
                var tokenExpiry = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                if (tokenExpiry < DateTime.UtcNow)
                {
                    response.data = new { message = "Token expired" };
                    response.status_code = StatusCodes.Status401Unauthorized.ToString();
                    context.Result = new UnauthorizedObjectResult(response);
                    return;
                }
            }
        }
    }
}
