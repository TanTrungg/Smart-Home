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
    [Route("api/survey-reports")]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        private readonly ISurveyService _surveyService;

        public SurveyController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListViewModel<SurveyViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all surveys.")]
        public async Task<ActionResult<ListViewModel<SurveyViewModel>>> GetSurveys([FromQuery] SurveyFilterModel filter, [FromQuery] PaginationRequestModel pagination)
        {
            return await _surveyService.GetSurveys(filter, pagination);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(SurveyViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get survey by id.")]
        public async Task<ActionResult<SurveyViewModel>> GetSurvey([FromRoute] Guid id)
        {
            return await _surveyService.GetSurvey(id);
        }

        [HttpPost]
        [ProducesResponseType(typeof(SurveyViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Create survey.")]
        public async Task<ActionResult<SurveyViewModel>> CreateSurvey([FromBody][Required] CreateSurveyModel model)
        {
            var survey = await _surveyService.CreateSurvey(model);
            return CreatedAtAction(nameof(GetSurvey), new { id = survey.Id }, survey);
        }

        [HttpPut]
        [Route("{id}")]
        //[Authorize(AccountRole.Teller)]
        [ProducesResponseType(typeof(SurveyViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Update survey.")]
        public async Task<ActionResult<SurveyViewModel>> UpdateSurvey([FromRoute] Guid id, [FromBody] UpdateSurveyModel model)
        {
            var survey = await _surveyService.UpdateSurvey(id, model);
            return CreatedAtAction(nameof(GetSurvey), new { id = survey.Id }, survey);
        }
    }
}
