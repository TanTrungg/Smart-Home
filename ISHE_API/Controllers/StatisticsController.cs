using ISHE_Data.Models.Views;
using ISHE_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ISHE_API.Controllers
{
    [Route("api/statistics")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IDevicePackageService _devicePackageService;
        public StatisticsController(IPaymentService paymentService, IDevicePackageService devicePackageService)
        {
            _paymentService = paymentService;
            _devicePackageService = devicePackageService;
        }

        [HttpGet]
        [Route("revenues")]
        [ProducesResponseType(typeof(List<PaymentViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get revenues")]
        public async Task<ActionResult<List<PaymentViewModel>>> GetRevenues([FromQuery] int? month)
        {
            return await _paymentService.GetRevenues(month);
        }

        [HttpGet]
        [Route("most-packages")]
        [ProducesResponseType(typeof(List<MostDevicePackageViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get most packages")]
        public async Task<ActionResult<List<MostDevicePackageViewModel>>> GetMostPackage()
        {
            return await _devicePackageService.GetMostDevicePackages();
        }
    }
}
