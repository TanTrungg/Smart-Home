using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Get;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;
using Microsoft.AspNetCore.Http;

namespace ISHE_Service.Interfaces
{
    public interface IStaffService
    {
        Task<ListViewModel<StaffViewModel>> GetStaffs(StaffFilterModel filter, PaginationRequestModel pagination);
        Task<ListViewModel<StaffGroupViewModel>> GetStaffLeads(StaffFilterModel filter, PaginationRequestModel pagination);
        Task<List<StaffViewModel>> GetStaffsAvaiableForSurey(StaffLeadRequestModel model);
        Task<StaffViewModel> GetStaff(Guid id);
        Task<StaffViewModel> CreateStaff(RegisterStaffModel model);
        Task<StaffViewModel> UpdateStaff(Guid id, UpdateStaffModel model);
        Task<StaffViewModel> UploadAvatar(Guid id, IFormFile image);

    }
}
