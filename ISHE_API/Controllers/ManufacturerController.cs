using ISHE_API.Configurations.Middleware;
using ISHE_Data.Models.Internal;
using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Views;
using ISHE_Service.Interfaces;
using ISHE_Utility.Constants;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ISHE_API.Controllers
{
    [Route("api/manufacturers")]
    [ApiController]
    public class ManufacturerController : ControllerBase
    {
        private readonly IManufacturerService _manufacturerService;

        public ManufacturerController(IManufacturerService manufacturerService)
        {
            _manufacturerService = manufacturerService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ManufacturerViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all manufacturers.")]
        public async Task<ActionResult<List<ManufacturerViewModel>>> GetManufacturers([FromQuery] ManufacturerFilterModel filter)
        {
            return await _manufacturerService.GetManufacturers(filter);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ManufacturerViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get manufacturer by id.")]
        public async Task<ActionResult<ManufacturerViewModel>> GetManufacturer([FromRoute] Guid id)
        {
            return await _manufacturerService.GetManufacturer(id);
        }

        [HttpPost]
        [Authorize(AccountRole.Owner)]
        [ProducesResponseType(typeof(ManufacturerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [SwaggerOperation(Summary = "Create manufacturer.")]
        public async Task<ActionResult<ManufacturerViewModel>> CreateManufacturer([FromBody][Required] CreateManufacturerModel model)
        {
            var manufacturer = await _manufacturerService.CreateManufacturer(model);
            return CreatedAtAction(nameof(GetManufacturer), new { id = manufacturer.Id }, manufacturer);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(AccountRole.Owner)]
        [ProducesResponseType(typeof(ManufacturerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Update manufacturer.")]
        public async Task<ActionResult<ManufacturerViewModel>> UpdateManufacturer([FromRoute] Guid id, [FromBody] CreateManufacturerModel model)
        {
            var manufacturer = await _manufacturerService.UpdateManufacturer(id, model);
            return CreatedAtAction(nameof(GetManufacturer), new { id = manufacturer.Id }, manufacturer);
        }
    }
}
