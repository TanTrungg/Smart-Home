﻿using ISHE_API.Configurations.Middleware;
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
        [ProducesResponseType(typeof(ListViewModel<ContractModificationViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all contract modify.")]
        public async Task<ActionResult<ListViewModel<ContractModificationViewModel>>> GetContractModifications([FromQuery] ContractModificationFilterModel filter, [FromQuery] PaginationRequestModel pagination)
        {
            return await _modificationService.GetContractModifications(filter, pagination);
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
        //  [Authorize(AccountRole.Customer)]
        [ProducesResponseType(typeof(ContractModificationViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Create contract modify.")]
        public async Task<ActionResult<ContractModificationViewModel>> CreateContractModification([FromBody][Required] CreateContractModificationModel model)
        {
            var mod = await _modificationService.CreateContractModification(model);
            return CreatedAtAction(nameof(GetContractModification), new { id = mod.Id }, mod);
        }

        [HttpPut]
        [Route("{id}")]
        //[Authorize(AccountRole.Teller)]
        [ProducesResponseType(typeof(ContractModificationViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Update contract modify.")]
        public async Task<ActionResult<ContractModificationViewModel>> UpdateContractModification([FromRoute] Guid id, [FromBody] UpdateContractModificationModel model)
        {
            var mod = await _modificationService.UpdateContractModification(id, model);
            return CreatedAtAction(nameof(GetContractModification), new { id = mod.Id }, mod);
        }
    }
}
