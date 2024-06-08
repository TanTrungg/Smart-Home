using ISHE_Data.Models.Requests.Post;
using ISHE_Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ISHE_API.Controllers
{
    [Route("api/send-mail")]
    [ApiController]
    public class SendMailController : ControllerBase
    {
        private readonly ISendMailService _sendMailService;

        public SendMailController(ISendMailService sendMailService)
        {
            _sendMailService = sendMailService;
        }

        [HttpPost]
        public async Task<IActionResult> TestSendMail([FromBody] CreateMailInfoRequest model)
        {
            await _sendMailService.SendEmail(model.Email, model.Title, model.Message);
            return Ok();
        }
    }

    
}
