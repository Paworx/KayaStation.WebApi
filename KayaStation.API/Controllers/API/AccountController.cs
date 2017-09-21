using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using KayaStation.DAL.Models;
using Microsoft.Extensions.Logging;
using KayaStation.API.Models.AccountViewModels;

namespace KayaStation.API.Controllers.API
{
    [Produces("application/json")]
    [Route("api/v1/Account")]
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
    }
}