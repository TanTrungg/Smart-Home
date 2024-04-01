using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Get;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;

namespace ISHE_Service.Interfaces
{
    public interface ISurveyService
    {
        Task<ListViewModel<SurveyViewModel>> GetSurveys(SurveyFilterModel filter, PaginationRequestModel pagination);
        Task<SurveyViewModel> GetSurvey(Guid id);
        Task<SurveyViewModel> CreateSurvey(CreateSurveyModel model);
        Task<SurveyViewModel> UpdateSurvey(Guid id, UpdateSurveyModel model);
    }
}
