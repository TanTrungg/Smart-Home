using ISHE_Data.Models.Views;
using ISHE_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ISHE_API.Controllers
{
    [Route("api/revenues")]
    [ApiController]
    public class RevenueController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public RevenueController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PaymentViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get revenues")]
        public async Task<ActionResult<List<PaymentViewModel>>> GetRevenues([FromQuery] int? year)
        {
            return await _paymentService.GetRevenues(year);
        }
    }
}
