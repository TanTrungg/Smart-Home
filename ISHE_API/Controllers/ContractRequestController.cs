using ISHE_Data.Models.Internal;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;
using ISHE_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ISHE_API.Controllers
{
    [Route("api/contract-requests")]
    [ApiController]
    public class ContractRequestController : ControllerBase
    {
        private readonly IContractModificationService _modificationService;

        public ContractRequestController(IContractModificationService modificationService)
        {
            _modificationService = modificationService;
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ContractModificationViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get contract modify by id.")]
        public async Task<ActionResult<ContractModificationViewModel>> GetContractModification([FromRoute] Guid id)
        {
            return await _modificationService.GetContractModification(id);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ContractModificationViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Create survey.")]
        public async Task<ActionResult<ContractModificationViewModel>> CreateSurvey([FromBody][Required] CreateContractModificationModel model)
        {
            var survey = await _modificationService.CreateContractModification(model);
            return CreatedAtAction(nameof(GetContractModification), new { id = survey.Id }, survey);
        }

        [HttpPut]
        [Route("{id}")]
        //[Authorize(AccountRole.Teller)]
        [ProducesResponseType(typeof(ContractModificationViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Update survey.")]
        public async Task<ActionResult<ContractModificationViewModel>> UpdateSurvey([FromRoute] Guid id, [FromBody] UpdateContractModificationModel model)
        {
            var survey = await _modificationService.UpdateContractModification(id, model);
            return CreatedAtAction(nameof(GetContractModification), new { id = survey.Id }, survey);
        }
    }
}
