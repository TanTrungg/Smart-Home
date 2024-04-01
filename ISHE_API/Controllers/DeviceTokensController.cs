using ISHE_API.Configurations.Middleware;
using ISHE_Data.Models.Internal;
using ISHE_Data.Models.Requests.Post;
using ISHE_Service.Interfaces;
using ISHE_Utility.Constants;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ISHE_API.Controllers
{
    [Route("api/device-tokens")]
    [ApiController]
    public class DeviceTokensController : ControllerBase
    {
        private readonly IDeviceTokenService _deviceTokenService;

        public DeviceTokensController(IDeviceTokenService deviceTokenService)
        {
            _deviceTokenService = deviceTokenService;
        }

        [HttpPost]
        [Authorize(AccountRole.Staff, AccountRole.Owner, AccountRole.Teller, AccountRole.Customer)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [SwaggerOperation(Summary = "Create new device token for account.")]
        public async Task<ActionResult<bool>> CreateDeviceToken([FromBody] CreateDeviceTokenModel model)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            return await _deviceTokenService.CreateDeviceToken(auth!.Id, model);
        }
    }
}
