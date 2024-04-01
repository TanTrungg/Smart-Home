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
    [Route("api/promotions")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListViewModel<PromotionViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all promotions.")]
        public async Task<ActionResult<ListViewModel<PromotionViewModel>>> GetPromotions([FromQuery] PromotionFilterModel filter, [FromQuery] PaginationRequestModel pagination)
        {
            return await _promotionService.GetPromotions(filter, pagination);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(PromotionDetailViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get promotion by id.")]
        public async Task<ActionResult<PromotionDetailViewModel>> GetPromotion([FromRoute] Guid id)
        {
            return await _promotionService.GetPromotion(id);
        }

        [HttpPost]
        [Authorize(AccountRole.Owner)]
        [ProducesResponseType(typeof(PromotionDetailViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Create promotion.")]
        public async Task<ActionResult<SmartDeviceDetailViewModel>> CreatePromotion([FromForm][Required] CreatePromotionModel model)
        {
            var promotion = await _promotionService.CreatePromotion(model);
            return CreatedAtAction(nameof(GetPromotion), new { id = promotion.Id }, promotion);
        }

        [HttpPut]
        [Authorize(AccountRole.Owner)]
        [Route("{id}")]
        [ProducesResponseType(typeof(PromotionDetailViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Update promotion.")]
        public async Task<ActionResult<SmartDeviceDetailViewModel>> UpdatePromotion([FromRoute] Guid id, [FromForm] UpdatePromotionModel model)
        {
            var promotion = await _promotionService.UpdatePromotion(id, model);
            return CreatedAtAction(nameof(GetPromotion), new { id = promotion.Id }, promotion);
        }
    }
}
