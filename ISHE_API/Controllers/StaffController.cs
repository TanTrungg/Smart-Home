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
    [Route("api/staffs")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListViewModel<StaffViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all staff accounts.")]
        public async Task<ActionResult<ListViewModel<StaffViewModel>>> GetStaffs([FromQuery] StaffFilterModel filter, [FromQuery] PaginationRequestModel pagination)
        {
            return await _staffService.GetStaffs(filter, pagination);
        }

        [HttpGet]
        [Route("leader")]
        [ProducesResponseType(typeof(ListViewModel<StaffGroupViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all staff lead accounts.")]
        public async Task<ActionResult<ListViewModel<StaffGroupViewModel>>> GetStaffLeads([FromQuery] StaffFilterModel filter, [FromQuery] PaginationRequestModel pagination)
        {
            return await _staffService.GetStaffLeads(filter, pagination);
        }

        [HttpGet]
        [Route("leader-avaiable")]
        [ProducesResponseType(typeof(List<StaffViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all staff lead for survey.")]
        public async Task<ActionResult<List<StaffViewModel>>> GetStaffsAvaiableForSurey([FromQuery][Required] StaffLeadRequestModel request)
        {
            return await _staffService.GetStaffsAvaiableForSurey(request);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(StaffViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Get staff by id.")]
        public async Task<ActionResult<StaffViewModel>> GetStaff([FromRoute] Guid id)
        {
            return await _staffService.GetStaff(id);
        }

        [HttpPost]
        [ProducesResponseType(typeof(StaffViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status409Conflict)]
        [SwaggerOperation(Summary = "Register staff.")]
        public async Task<ActionResult<StaffViewModel>> CreateStaff([FromBody][Required] RegisterStaffModel model)
        {
            var staff = await _staffService.CreateStaff(model);
            return CreatedAtAction(nameof(GetStaff), new { id = staff.AccountId }, staff);
        }


        [HttpPut]
        [Route("{id}")]
        //[Authorize(UserRole.Staff)]
        [ProducesResponseType(typeof(StaffViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [SwaggerOperation(Summary = "Update staff.")]
        public async Task<ActionResult<StaffViewModel>> UpdateStaff([FromRoute] Guid id, [FromBody] UpdateStaffModel model)
        {
            var staff = await _staffService.UpdateStaff(id, model);
            return CreatedAtAction(nameof(GetStaff), new { id = staff.AccountId }, staff);
        }

        [HttpPut]
        [Route("avatar")]
        [Authorize(AccountRole.Staff)]
        [ProducesResponseType(typeof(StaffViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Upload avatar for staff.")]
        public async Task<ActionResult<StaffViewModel>> UploadAvatar([Required] IFormFile image)
        {
            var auth = (AuthModel?)HttpContext.Items["User"];
            var staff = await _staffService.UploadAvatar(auth!.Id, image);
            return CreatedAtAction(nameof(GetStaff), new { id = staff.AccountId }, staff);
        }
    }
}
