using COLLECTION_MANAGEMENT_API.Attributes;
using COLLECTION_MANAGEMENT_API.ViewModels;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using COLLECTION_MANAGEMENT_REPOSITORY.UoW;
using COLLECTION_MANAGEMENT_SERVICE.Interface;
using COLLECTION_MANAGEMENT_UTILITY;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using ILogger = Serilog.ILogger;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net;
using COLLECTION_MANAGEMENT_SERVICE.Manager;
using Newtonsoft.Json;
using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;

namespace COLLECTION_MANAGEMENT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<long>> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICommonManager _commonManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<long>> roleManager, IConfiguration configuration, SignInManager<ApplicationUser> signInManager, ICommonManager commonManager, IUnitOfWork unitOfWork, ILogger logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _commonManager = commonManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [TypeFilter(typeof(ValidationFilterAttribute), Order = 1)]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            CommonResponse response = new();
            try
            {

                var user = new ApplicationUser
                {
                    UserName = model.email,
                    Email = model.email,
                    FullName = model.full_name ?? string.Empty,
                    ContactNo = model.contact_no
                };

                var result = await _userManager.CreateAsync(user, model.password);

                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                // Assign role if provided
                if (!string.IsNullOrEmpty(model.role))
                {
                    if (!await _unitOfWork.Roles.RoleExistsAsync(model.role))
                        await _roleManager.CreateAsync(new IdentityRole<long> { Name = model.role, NormalizedName = model.role });

                    await _userManager.AddToRoleAsync(user, model.role);
                }
                response = await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"AccountController/Register ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                response = await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response);
            }
            return Ok(response);
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("get-users")]
        public async Task<IActionResult> GetAll(int page, int pageSize)
        {
            page = page > 1 ? page : 1;
            pageSize = pageSize > 0 ? pageSize : 10;
            _logger.Information($"DiagnosticCodeController/GetAll ==> request entity: page: {page}, pageSize: {pageSize}");
            _unitOfWork.SetActiveContext(CommonEnum.ContextName.Identity);
            var allUsers = await _unitOfWork.Users.GetAllPagedAsync(page, pageSize);
            CommonResponse response = new();
            _logger.Information($"DiagnosticCodeController/GetAll ==> response entity: {WebUtility.HtmlEncode(JsonConvert.SerializeObject(response, Formatting.None))}");
            return Ok(response);
        }

        [TypeFilter(typeof(ValidationFilterAttribute), Order = 1)]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            CommonResponse response = new();
            try
            {
                var user = await _userManager.FindByEmailAsync(model.email);
                if (user == null || string.IsNullOrWhiteSpace(user.Email))
                {
                    response = await _commonManager.HandleResponse(StatusCodes.Status401Unauthorized, (int)CommonEnum.ResponseCodes.IncorrectUsernameOrPassword, response);
                    return Ok(response);
                }

                //var result = await _signInManager.CheckPasswordSignInAsync(user, model.password, true);
                var result = await _signInManager.PasswordSignInAsync(user.Email, model.password, false, true);

                if (result.RequiresTwoFactor)
                {
                    return Ok(await _commonManager.HandleResponse(StatusCodes.Status206PartialContent, (int)CommonEnum.ResponseCodes.TwoFactorRequired, response));
                }

                if (!result.Succeeded)
                    return Ok(await _commonManager.HandleResponse(StatusCodes.Status401Unauthorized, (int)CommonEnum.ResponseCodes.IncorrectUsernameOrPassword, response));

                response = await GenerateJwtToken(user);
                if (!string.IsNullOrWhiteSpace(response.status_code) && response.status_code != ((int)StatusCodes.Status200OK).ToString())
                {
                    return Ok(response);
                }
                return Ok(await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response));
            }
            catch (Exception ex)
            {
                _logger.Error($"AccountController/Login ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return Ok(await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.InvalidRequest, response));
            }
        }

        [HttpPost("verify-2fa-login")]
        public async Task<IActionResult> Verify2FALogin([FromBody] Verify2FARequestEntity model)
        {
            CommonResponse response = new();
            try
            {
                if (string.IsNullOrWhiteSpace(model.email))
                {
                    return Ok(await _commonManager.HandleResponse(StatusCodes.Status401Unauthorized, (int)CommonEnum.ResponseCodes.EmailIsEmpty, response));
                }
                var user = await _userManager.FindByEmailAsync(model.email);
                if (user == null)
                {
                    return Ok(await _commonManager.HandleResponse(StatusCodes.Status401Unauthorized, (int)CommonEnum.ResponseCodes.Unauthorized, response));
                }

                //var is2faValid = await _signInManager.TwoFactorAuthenticatorSignInAsync(model.code, false, false);
                var is2faValid = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, model.code);
                //if (!is2faValid.Succeeded)
                //{
                //    return Ok(await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.InvalidCode, response));
                //}
                if (!is2faValid)
                {
                    return Ok(await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.InvalidCode, response));
                }

                response = await GenerateJwtToken(user);
                return Ok(await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response));
            }
            catch (Exception ex)
            {
                _logger.Error($"AccountController/Verify2FALogin ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return Ok(await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response));
            }
        }

        private async Task<CommonResponse> GenerateJwtToken(ApplicationUser user)
        {
            CommonResponse response = new();

            try
            {
                int expiry_minutes = _configuration.GetValue<int>("JWT:ExpiryMinutes");
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Count == 0)
                {
                    _logger.Information($"AccountController/GenerateJwtToken ==> role not assigned for user {user.UserName}");
                    return await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.RoleNotFound, response);
                }
                var claims = new List<Claim>
                {
                    new Claim("user_id", user.Id.ToString()), // Set Name claim directly
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("role_name", roles[0])
                };

                
                //claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:Key") ?? string.Empty);

                SecurityTokenDescriptor tokenDescriptor = new()
                {
                    Subject = new ClaimsIdentity(claims),
                    NotBefore = DateTime.Now,
                    Expires = DateTime.Now.AddMinutes(expiry_minutes),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                };
                string expiryTime = tokenDescriptor.Expires.Value.ToString("yyyy-MM-dd HH:mm:ss");
                SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
                string bearer_token = tokenHandler.WriteToken(token);

                BearerTokenResponseEntity bearerTokenResponseData = new()
                {
                    token = bearer_token,
                    expiry_time = expiryTime
                };
                response.status_code = ((int)CommonEnum.ResponseCodes.Success).ToString();
                response.data = bearerTokenResponseData;
            }
            catch (Exception ex)
            {

                throw;
            }

            return response;
        }


        [TypeFilter(typeof(ValidationFilterAttribute), Order = 1)]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            CommonResponse response = new();
            try
            {
                var user = await _userManager.FindByEmailAsync(model.email);
                if (user == null)
                {
                    return Ok(await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.UserNotFound, response));
                }

                string? token = await _userManager.GeneratePasswordResetTokenAsync(user);

                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.Information($"AccountController/ForgotPassword ==> token generation for new password failed.");
                    return Ok(await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.ResetPasswordTokenEmpty, response));
                }

                // For dev/testing: return token in response (later send via email)
                response.data = new { token = token };
                return Ok(await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response));
            }
            catch (Exception ex)
            {
                _logger.Error($"AccountController/ForgotPassword ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return Ok(await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response));
            }
        }


        [TypeFilter(typeof(ValidationFilterAttribute), Order = 1)]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            CommonResponse response = new();
            try
            {
                var user = await _userManager.FindByEmailAsync(model.email);
                if (user == null)
                {
                    return Ok(await _commonManager.HandleResponse(StatusCodes.Status404NotFound, (int)CommonEnum.ResponseCodes.NotFound, response));
                }

                var result = await _userManager.ResetPasswordAsync(user, model.token, model.new_password);
                if (!result.Succeeded)
                {
                    var firstError = result.Errors.FirstOrDefault()?.Description ?? "Failed to reset password.";
                    _logger.Information($"AccountController/ResetPassword ==> Error: {JsonConvert.SerializeObject(result.Errors, Formatting.None)}");
                    return Ok(await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.FailedToReset, response, string.Empty, firstError));
                }

                return Ok(await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response));
            }
            catch (Exception ex)
            {
                _logger.Error($"AccountController/ResetPassword ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return Ok(await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response));
            }
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet("enable-2fa")]
        public async Task<IActionResult> Enable2FA()
        {
            CommonResponse response = new();
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null || string.IsNullOrWhiteSpace(user.Email))
                    return Ok(await _commonManager.HandleResponse(StatusCodes.Status401Unauthorized, (int)CommonEnum.ResponseCodes.Unauthorized, response));

                var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
                if (string.IsNullOrEmpty(unformattedKey))
                {
                    await _userManager.ResetAuthenticatorKeyAsync(user);
                    unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
                }
                if (string.IsNullOrWhiteSpace(unformattedKey))
                {
                    return Ok(await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UnprocessableEntity, response));
                }
                string sharedKey = CommonHelper.FormatKey(unformattedKey);
                string authenticatorUri = CommonHelper.GenerateQrCodeUri(user.Email, unformattedKey);

                response.data = new
                {
                    shared_key = sharedKey,
                    auth_uri = authenticatorUri,
                };

                return Ok(await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response));
            }
            catch (Exception ex)
            {
                _logger.Error($"AccountController/Enable2FA ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return Ok(await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response));
            }
        }

        [TypeFilter(typeof(ValidationFilterAttribute), Order = 1)]
        [HttpPost("verify-2fa")]
        public async Task<IActionResult> Verify2FA([FromBody] Verify2FARequestEntity model)
        {
            CommonResponse response = new();
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Ok(await _commonManager.HandleResponse(StatusCodes.Status401Unauthorized, (int)CommonEnum.ResponseCodes.Unauthorized, response));

                var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, model.code);
                if (!isValid)
                    return Ok(await _commonManager.HandleResponse(StatusCodes.Status401Unauthorized, (int)CommonEnum.ResponseCodes.InvalidCode, response));

                await _userManager.SetTwoFactorEnabledAsync(user, true);
                return Ok(await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response));
            }
            catch (Exception ex)
            {
                _logger.Error($"AccountController/Verify2FA ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return Ok(await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response));
            }
        }

        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpPost("disable-2fa")]
        public async Task<IActionResult> Disable2FA()
        {
            CommonResponse response = new();
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Ok(await _commonManager.HandleResponse(StatusCodes.Status401Unauthorized, (int)CommonEnum.ResponseCodes.Unauthorized, response));

                var disableResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
                if (!disableResult.Succeeded)
                {
                    var error = disableResult.Errors.FirstOrDefault()?.Description ?? "Failed to disable 2FA.";
                    return Ok(await _commonManager.HandleResponse(StatusCodes.Status422UnprocessableEntity, (int)CommonEnum.ResponseCodes.UnprocessableEntity, response, string.Empty, error));
                }

                return Ok(await _commonManager.HandleResponse(StatusCodes.Status200OK, (int)CommonEnum.ResponseCodes.Success, response));
            }
            catch (Exception ex)
            {
                _logger.Error($"AccountController/Disable2FA ==> Error: {WebUtility.HtmlEncode(ex.ToString())}");
                return Ok(await _commonManager.HandleResponse(StatusCodes.Status500InternalServerError, (int)CommonEnum.ResponseCodes.InternalServerError, response));
            }
        }


        [TypeFilter(typeof(AuthorizationFilterAttribute), Order = 1)]
        [HttpGet]
        [Route("token-testing")]
        public async Task<IActionResult> TokenTesting()
        {
            CommonResponse response = new()
            {
                data = new { message = "hello" },
                status_code = StatusCodes.Status200OK.ToString()
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("token-validate")]
        public bool ValidateToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:Key") ?? string.Empty);
                var parameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = JwtRegisteredClaimNames.Name
                };

                var principal = handler.ValidateToken(token, parameters, out _);
                return principal != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Token validation failed: " + ex.Message);
                return false;
            }
        }
    }
}
