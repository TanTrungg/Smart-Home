using ISHE_Data.Models.Requests.Post;
using ISHE_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ISHE_API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        [Route("zalo-pay")]
        [SwaggerOperation(Summary = "ZaloPay payment.")]
        public async Task<IActionResult> CreateZaloPay([FromBody] CreatePaymentModel model)
        {
            return Ok(await _paymentService.ProcessZalopayPayment(model));
        }

        [HttpPost]
        [Route("zalopay-callback")]
        [SwaggerOperation(Summary = "ZaloPay callback.")]
        public async Task<IActionResult> Post([FromBody] dynamic cbdata)
        {
            return Ok(await _paymentService.IsValidCallback(cbdata));
        }
    }
}
