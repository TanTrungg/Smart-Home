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
    [Route("api/smart-devices")]
    [ApiController]
    public class SmartDeviceController : ControllerBase
    {
        private readonly ISmartDeviceService _smartDeviceService;

        public SmartDeviceController(ISmartDeviceService smartDeviceService)
        {
            _smartDeviceService = smartDeviceService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListViewModel<SmartDeviceDetailViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all smart devices.")]
        public async Task<ActionResult<ListViewModel<SmartDeviceDetailViewModel>>> GetSmartDevices([FromQuery] SmartDeviceFilterModel filter, [FromQuery] PaginationRequestModel pagination)
        {
            return await _smartDeviceService.GetSmartDevices(filter, pagination);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(SmartDeviceDetailViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get smart device by id.")]
        public async Task<ActionResult<SmartDeviceDetailViewModel>> GetSmartDevice([FromRoute] Guid id)
        {
            return await _smartDeviceService.GetSmartDevice(id);
        }

        [HttpPost]
        [Authorize(AccountRole.Owner)]
        [ProducesResponseType(typeof(SmartDeviceDetailViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Create smart device.")]
        public async Task<ActionResult<SmartDeviceDetailViewModel>> CreateSmartDevice([FromForm][Required] CreateSmartDeviceModel model)
        {
            var smartDevice = await _smartDeviceService.CreateSmartDevice(model);
            return CreatedAtAction(nameof(GetSmartDevice), new { id = smartDevice.Id }, smartDevice);
        }

        [HttpPut]
        [Authorize(AccountRole.Owner)]
        [Route("{id}")]
        [ProducesResponseType(typeof(SmartDeviceDetailViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Update smart device.")]
        public async Task<ActionResult<SmartDeviceDetailViewModel>> UpdateSmartDevice([FromRoute] Guid id, [FromForm] UpdateSmartDeviceModel model)
        {
            var smartDevice = await _smartDeviceService.UpdateSmartDevice(id, model);
            return CreatedAtAction(nameof(GetSmartDevice), new { id = smartDevice.Id }, smartDevice);
        }

        [HttpPut]
        [Authorize(AccountRole.Owner)]
        [Route("image/{id}")]
        [ProducesResponseType(typeof(SmartDeviceDetailViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Update smart device image.")]
        public async Task<ActionResult<SmartDeviceDetailViewModel>> UpdateSmartDeviceImage([FromRoute] Guid id, [FromForm] UpdateImageModel model)
        {
            var smartDevice = await _smartDeviceService.UpdateSmartDeviceImage(id, model);
            return CreatedAtAction(nameof(GetSmartDevice), new { id = smartDevice.Id }, smartDevice);
        }

        [HttpDelete]
        [Authorize(AccountRole.Owner)]
        [Route("image")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Remove repair service image.")]
        public async Task<IActionResult> Remove([FromForm] List<Guid> ids)
        {
            await _smartDeviceService.RemoveSmartDeviceImage(ids);
            return NoContent();
        }
    }
}
