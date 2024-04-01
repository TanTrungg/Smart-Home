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
    [Route("api/customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListViewModel<CustomerViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all customers")]
        public async Task<ActionResult<ListViewModel<CustomerViewModel>>> GetCustomers([FromQuery] CustomerFilterModel filter, [FromQuery] PaginationRequestModel pagination)
        {
            return await _customerService.GetCustomers(filter, pagination);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get customer by id.")]
        public async Task<ActionResult<CustomerViewModel>> GetCustomer([FromRoute] Guid id)
        {
            return await _customerService.GetCustomer(id);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [SwaggerOperation(Summary = "Register customer.")]
        public async Task<ActionResult<OwnerViewModel>> CreateCustomer([FromBody][Required] RegisterCustomerModel model)
        {
            var customer = await _customerService.CreateCustomer(model);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.AccountId }, customer);
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Update customer.")]
        public async Task<ActionResult<OwnerViewModel>> UpdateCustomer([FromRoute] Guid id, [FromBody] UpdateCustomerModel model)
        {
            var customer = await _customerService.UpdateCustomer(id, model);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.AccountId }, customer);
        }

        [HttpPut]
        [Route("avatar")]
        [Authorize(AccountRole.Customer)]
        [ProducesResponseType(typeof(CustomerViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Upload avatar for customer.")]
        public async Task<ActionResult<OwnerViewModel>> UploadAvatar([Required] IFormFile image)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var customer = await _customerService.UploadAvatar(auth!.Id, image);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.AccountId }, customer);
        }
    }
}
