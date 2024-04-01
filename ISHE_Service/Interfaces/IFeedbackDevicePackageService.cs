using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;

namespace ISHE_Service.Interfaces
{
    public interface IFeedbackDevicePackageService
    {
        Task<FeedbackDevicePackageViewModel> GetFeedback(Guid id);
        Task<FeedbackDevicePackageViewModel> CreateFeedback(CreateFeedbackDevicePackageModel model);
        Task<FeedbackDevicePackageViewModel> UpdateFeedBack(Guid id, UpdateFeedbackDevicePackageModel model);
    }
}
