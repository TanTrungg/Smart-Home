using ISHE_API.Configurations.Middleware;
using ISHE_Data.Models.Internal;
using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Get;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;
using ISHE_Service.Interfaces;
using ISHE_Utility.Constants;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ISHE_API.Controllers
{
    [Route("api/tellers")]
    [ApiController]
    public class TellerController : ControllerBase
    {
        private readonly ITellerService _tellerService;

        public TellerController(ITellerService tellerService)
        {
            _tellerService = tellerService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListViewModel<TellerViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all teller accounts.")]
        public async Task<ActionResult<ListViewModel<TellerViewModel>>> GetTellers([FromQuery] TellerFilterModel filter, [FromQuery] PaginationRequestModel pagination)
        {
            return await _tellerService.GetTellers(filter, pagination);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(TellerViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get teller by id.")]
        public async Task<ActionResult<TellerViewModel>> GetTeller([FromRoute] Guid id)
        {
            return await _tellerService.GetTeller(id);
        }

        [HttpPost]
        [ProducesResponseType(typeof(TellerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [SwaggerOperation(Summary = "Register teller.")]
        public async Task<ActionResult<TellerViewModel>> CreateTeller([FromBody][Required] RegisterTellerModel model)
        {
            var teller = await _tellerService.CreateTeller(model);
            //chuẩn REST
            return CreatedAtAction(nameof(GetTeller), new { id = teller.AccountId }, teller);
        }

        [HttpPut]
        [Route("{id}")]
        //[Authorize(AccountRole.Teller)]
        [ProducesResponseType(typeof(TellerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Update teller.")]
        public async Task<ActionResult<TellerViewModel>> UpdateTeller([FromRoute] Guid id, [FromBody] UpdateTellerModel model)
        {
            var teller = await _tellerService.UpdateTeller(id, model);
            return CreatedAtAction(nameof(GetTeller), new { id = teller.AccountId }, teller);
        }

        [HttpPut]
        [Route("avatar")]
        [Authorize(AccountRole.Teller)]
        [ProducesResponseType(typeof(TellerViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Upload avatar for teller.")]
        public async Task<ActionResult<TellerViewModel>> UploadAvatar([Required] IFormFile image)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var teller = await _tellerService.UploadAvatar(auth!.Id, image);
            return CreatedAtAction(nameof(GetTeller), new { id = teller.AccountId }, teller);
        }
    }
}
