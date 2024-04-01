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
    [Route("api/device-packages")]
    [ApiController]
    public class DevicePackageController : ControllerBase
    {
        private readonly IDevicePackageService _devicePackageService;

        public DevicePackageController(IDevicePackageService devicePackageService)
        {
            _devicePackageService = devicePackageService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListViewModel<DevicePackageViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all device packages.")]
        public async Task<ActionResult<ListViewModel<DevicePackageViewModel>>> GetDevicePackages([FromQuery] DevicePackageFilterModel filter, [FromQuery] PaginationRequestModel pagination)
        {
            return await _devicePackageService.GetDevicePackages(filter, pagination);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(DevicePackageDetailViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get device package by id.")]
        public async Task<ActionResult<DevicePackageDetailViewModel>> GetDevicePackage([FromRoute] Guid id)
        {
            return await _devicePackageService.GetDevicePackage(id);
        }

        [HttpPost]
        [Authorize(AccountRole.Owner)]
        [ProducesResponseType(typeof(PromotionDetailViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Create device package.")]
        public async Task<ActionResult<DevicePackageDetailViewModel>> CreateDevicePackage([FromForm][Required] CreateDevicePackageModel model)
        {
            var package = await _devicePackageService.CreateDevicePackage(model);
            return CreatedAtAction(nameof(GetDevicePackage), new { id = package.Id }, package);
        }

        [HttpPut]
        [Authorize(AccountRole.Owner)]
        [Route("{id}")]
        [ProducesResponseType(typeof(DevicePackageDetailViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Update device package.")]
        public async Task<ActionResult<SmartDeviceDetailViewModel>> UpdateDevicePackage([FromRoute] Guid id, [FromForm] UpdateDevicePackageModel model)
        {
            var package = await _devicePackageService.UpdateDevicePackage(id, model);
            return CreatedAtAction(nameof(GetDevicePackage), new { id = package.Id }, package);
        }
    }
}
