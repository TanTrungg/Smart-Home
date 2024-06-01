using ISHE_Data.Models.Internal;
using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Get;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;
using ISHE_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ISHE_API.Controllers
{
    [Route("api/contracts")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListViewModel<PartialContractViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all contracts.")]
        public async Task<ActionResult<ListViewModel<PartialContractViewModel>>> GetContracts([FromQuery] ContractFilterModel filter, [FromQuery] PaginationRequestModel pagination)
        {
            return await _contractService.GetContracts(filter, pagination);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ContractViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get contract by id.")]
        public async Task<ActionResult<ContractViewModel>> GetContract([FromRoute] string id)
        {
            return await _contractService.GetContract(id);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ContractViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Create contract.")]
        public async Task<ActionResult<ContractViewModel>> CreateContract([FromBody][Required] CreateContractModel model)
        {
            var contract = await _contractService.CreateContract(model);
            return CreatedAtAction(nameof(GetContract), new { id = contract.Id }, contract);
        }

        [HttpPut]
        [Route("{id}")]
        //[Authorize(AccountRole.Teller)]
        [ProducesResponseType(typeof(ContractViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Update contract.")]
        public async Task<ActionResult<ContractViewModel>> UpdateContract([FromRoute] string id, [FromBody] UpdateContractModel model)
        {
            var contract = await _contractService.UpdateContract(id, model);
            return CreatedAtAction(nameof(GetContract), new { id = contract.Id }, contract);
        }

        [HttpPut]
        [Route("upload-image/{id}")]
        //[Authorize(AccountRole.Teller)]
        [ProducesResponseType(typeof(ContractViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Upload contract image.")]
        public async Task<ActionResult<ContractViewModel>> UploadContractImage([FromRoute] string id, [Required] IFormFile image)
        {
            var contract = await _contractService.UploadContractImage(id, image);
            return CreatedAtAction(nameof(GetContract), new { id = contract.Id }, contract);
        }

        [HttpPut]
        [Route("upload-acceptance/{id}")]
        //[Authorize(AccountRole.Teller)]
        [ProducesResponseType(typeof(ContractViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Upload contract acceptance.")]
        public async Task<ActionResult<ContractViewModel>> UploadContractAcceptance([FromRoute] string id, [Required] IFormFile image)
        {
            var contract = await _contractService.UploadContractAcceptance(id, image);
            return CreatedAtAction(nameof(GetContract), new { id = contract.Id }, contract);
        }
    }
}
