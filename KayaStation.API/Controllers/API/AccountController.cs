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
using KayaStation.Core.Data;
using KayaStation.API.Models;

namespace KayaStation.API.Controllers.API
{
    [Produces("application/json")]
    [Route("api/v1/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger logger;
        private readonly ApplicationDbContext db;


        public AccountController(
            UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger,
            ApplicationDbContext dbContext
            )
        {
            this.userManager = userManager;
            this.logger = logger;
            this.db = dbContext;
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

            var result = await userManager.CreateAsync(userIdentity, model.Password);

            if (!result.Succeeded) return new BadRequestObjectResult(ErrorHelper.AddErrorsToModelState(result, ModelState));

            if(model.IsHotelOwner) // OWNER ONLY
            {
                userIdentity.IsHotelOwner = true;
                userManager.UpdateAsync(userIdentity).Wait();

                var hotel = new Hotel();
                hotel.Name = model.HotelName;
                hotel.OwnerId = userIdentity.Id;

                db.Hotels.Add(hotel);
                db.SaveChanges();
            }

            return new OkResult();
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(Token), 200)]
        public async Task<IActionResult> AuthToken([FromBody]LoginViewModel model)
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
                                .AddExpiry(120)
                                .Build();
            var tokenRes = new Token { RequestToken = token.Value, ExpiresIn= token.ValidTo};
            return Ok(tokenRes);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                // get the user to verifty
                var userToVerify = await userManager.FindByNameAsync(userName);

                if (userToVerify != null)
                {
                    // check the credentials  
                    if (await userManager.CheckPasswordAsync(userToVerify, password))
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