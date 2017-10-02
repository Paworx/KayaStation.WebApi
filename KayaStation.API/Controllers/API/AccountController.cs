using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using KayaStation.Core.Models;
using Microsoft.Extensions.Logging;
using KayaStation.API.Models.AccountViewModels;
using System.Security.Claims;
using KayaStation.API.Auth;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;

namespace KayaStation.API.Controllers.API
{
    [Produces("application/json")]
    [Route("api/v1/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger
            )
        {
            _userManager = userManager;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdentity = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(userIdentity, model.Password);

            if (!result.Succeeded) return new BadRequestObjectResult(ErrorHelper.AddErrorsToModelState(result, ModelState));

            return new OkResult();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody]LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identity = await GetClaimsIdentity(model.Email, model.Password);
            if (identity == null)
            {
                return BadRequest(ErrorHelper.AddErrorToModelState("login_failure", "Invalid username or password.", ModelState));
            }
            
            var token = new JwtTokenBuilder()
                                .AddSecurityKey(JwtSecurityKey.Create("powerx-key-powerx-key-powerx-key-powerx-key"))
                                .AddSubject(identity.Name)
                                .AddIssuer("kayaStationIndentityServer")
                                .AddAudience("kayaStationIndentityClient")
                                .AddClaim("MembershipId", "111")
                                .AddExpiry(12)
                                .Build();
            var tokenResponse = new { token = token.Value, expiresIn= token.ValidTo};
            return Ok(tokenResponse);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                // get the user to verifty
                var userToVerify = await _userManager.FindByNameAsync(userName);

                if (userToVerify != null)
                {
                    // check the credentials  
                    if (await _userManager.CheckPasswordAsync(userToVerify, password))
                    {
                        return await Task.FromResult(GenerateClaimsIdentity(userName, userToVerify.Id));
                    }
                }
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }

        private ClaimsIdentity GenerateClaimsIdentity(string userName, string id)
        {
            return new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[]
            {
                new Claim(Constants.Strings.JwtClaimIdentifiers.Id, id),
                new Claim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess)
            });
        }
    }
}