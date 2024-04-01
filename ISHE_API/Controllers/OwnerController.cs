using ISHE_API.Configurations.Middleware;
using ISHE_Data.Models.Internal;
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
    [Route("api/owners")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerService _ownerService;

        public OwnerController(IOwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<OwnerViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all owner")]
        public async Task<ActionResult<List<OwnerViewModel>>> GetOwners()
        {
            return await _ownerService.GetOwners();
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(OwnerViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get owner by id.")]
        public async Task<ActionResult<OwnerViewModel>> GetOwner([FromRoute] Guid id)
        {
            return await _ownerService.GetOwner(id);
        }

        [HttpPost]
        [ProducesResponseType(typeof(OwnerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [SwaggerOperation(Summary = "Register owner.")]
        public async Task<ActionResult<OwnerViewModel>> CreateOwner([FromBody][Required] RegisterOwnerModel model)
        {
            var owner = await _ownerService.CreateOwner(model);
            //chuẩn REST
            return CreatedAtAction(nameof(GetOwner), new { id = owner.AccountId }, owner);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(AccountRole.Owner)]
        [ProducesResponseType(typeof(OwnerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Update owner.")]
        public async Task<ActionResult<OwnerViewModel>> UpdateOwner([FromRoute] Guid id, [FromBody] UpdateOwnerModel model)
        {
            var owner = await _ownerService.UpdateOwner(id, model);
            return CreatedAtAction(nameof(GetOwner), new { id = owner.AccountId }, owner);
        }

        [HttpPut]
        [Route("avatar")]
        [Authorize(AccountRole.Owner)]
        [ProducesResponseType(typeof(OwnerViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Upload avatar for owner.")]
        public async Task<ActionResult<OwnerViewModel>> UploadAvatar([Required] IFormFile image)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var owner = await _ownerService.UploadAvatar(auth!.Id, image);
            return CreatedAtAction(nameof(GetOwner), new { id = owner.AccountId }, owner);
        }
    }
}
