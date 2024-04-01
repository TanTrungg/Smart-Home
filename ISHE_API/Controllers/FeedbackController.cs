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
    [Route("api/feedbacks")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackDevicePackageService _feedbackService;

        public FeedbackController(IFeedbackDevicePackageService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(FeedbackDevicePackageViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get feedback by id.")]
        public async Task<ActionResult<FeedbackDevicePackageViewModel>> GetFeedback([FromRoute] Guid id)
        {
            return await _feedbackService.GetFeedback(id);
        }

        [HttpPost]
        [ProducesResponseType(typeof(FeedbackDevicePackageViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Create feedback.")]
        public async Task<ActionResult<FeedbackDevicePackageViewModel>> CreateFeedback([FromBody][Required] CreateFeedbackDevicePackageModel model)
        {
            var feedback = await _feedbackService.CreateFeedback(model);
            return CreatedAtAction(nameof(GetFeedback), new { id = feedback.Id }, feedback);
        }

        [HttpPut]
        [Route("{id}")]
        //[Authorize(AccountRole.Teller)]
        [ProducesResponseType(typeof(FeedbackDevicePackageViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Update feedback.")]
        public async Task<ActionResult<FeedbackDevicePackageViewModel>> UpdateFeedBack([FromRoute] Guid id, [FromBody] UpdateFeedbackDevicePackageModel model)
        {
            var feedback = await _feedbackService.UpdateFeedBack(id, model);
            return CreatedAtAction(nameof(GetFeedback), new { id = feedback.Id }, feedback);
        }
    }
}
