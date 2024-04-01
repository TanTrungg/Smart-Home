using ISHE_Data.Models.Internal;
using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Get;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;
using ISHE_Service.Implementations;
using ISHE_Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ISHE_API.Controllers
{
    [Route("api/survey-requests")]
    [ApiController]
    public class SurveyRequestController : ControllerBase
    {
        private readonly ISurveyRequestService _surveyRequestService;

        public SurveyRequestController(ISurveyRequestService surveyRequestService)
        {
            _surveyRequestService = surveyRequestService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListViewModel<SurveyRequestViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all survey requests.")]
        public async Task<ActionResult<ListViewModel<SurveyRequestViewModel>>> GetSurveyRequests([FromQuery] SurveyRequestFilterModel filter, [FromQuery] PaginationRequestModel pagination)
        {
            return await _surveyRequestService.GetSurveyRequests(filter, pagination);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(SurveyRequestViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get survey request by id.")]
        public async Task<ActionResult<SurveyRequestViewModel>> GetSurveyRequest([FromRoute] Guid id)
        {
            return await _surveyRequestService.GetSurveyRequest(id);
        }

        [HttpPost]
        [ProducesResponseType(typeof(SurveyRequestViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Create survey request.")]
        public async Task<ActionResult<SurveyRequestViewModel>> CreateSurveyRequest([FromBody][Required] CreateSurveyRequestModel model)
        {
            var request = await _surveyRequestService.CreateSurveyRequest(model);
            return CreatedAtAction(nameof(GetSurveyRequest), new { id = request.Id }, request);
        }

        [HttpPut]
        [Route("{id}")]
        //[Authorize(AccountRole.Teller)]
        [ProducesResponseType(typeof(SurveyRequestViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Update survey request.")]
        public async Task<ActionResult<SurveyRequestViewModel>> UpdateSurveyRequest([FromRoute] Guid id, [FromBody] UpdateSurveyRequestModel model)
        {
            var request = await _surveyRequestService.UpdateSurveyRequest(id, model);
            return CreatedAtAction(nameof(GetSurveyRequest), new { id = request.Id }, request);
        }
    }
}
