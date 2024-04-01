using ISHE_API.Configurations.Middleware;
using ISHE_Data.Models.Internal;
using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Get;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Views;
using ISHE_Service.Interfaces;
using ISHE_Utility.Constants;
using ISHE_Utility.Enum;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ISHE_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(AuthViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Login.")]
        public async Task<IActionResult> Authenticated([FromBody][Required] AuthRequest auth)
        {
            var token = await _accountService.Authenticated(auth);
            return Ok(token);
        }

        [HttpGet]
        [Authorize(AccountRole.Staff, AccountRole.Owner, AccountRole.Teller, AccountRole.Customer)]
        [ProducesResponseType(typeof(AccountViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get account login.")]
        public async Task<ActionResult<AccountViewModel>> GetAuth()
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var accounts = await _accountService.GetAccount(auth!.Id);
            return Ok(accounts);
        }
    }
}
