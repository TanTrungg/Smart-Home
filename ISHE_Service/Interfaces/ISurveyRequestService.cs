using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Get;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;

namespace ISHE_Service.Interfaces
{
    public interface ISurveyRequestService
    {
        Task<ListViewModel<SurveyRequestViewModel>> GetSurveyRequests(SurveyRequestFilterModel filter, PaginationRequestModel pagination);
        Task<SurveyRequestViewModel> GetSurveyRequest(Guid id);
        Task<SurveyRequestViewModel> CreateSurveyRequest(CreateSurveyRequestModel model);
        Task<SurveyRequestViewModel> UpdateSurveyRequest(Guid id, UpdateSurveyRequestModel model);
    }
}
