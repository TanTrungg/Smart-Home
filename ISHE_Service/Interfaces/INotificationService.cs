using ISHE_Data.Models.Requests.Get;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;

namespace ISHE_Service.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationViewModel> GetNotification(Guid id);
        Task<ListViewModel<NotificationViewModel>> GetNotifications(Guid accountId, PaginationRequestModel pagination);
        Task<List<string?>> GetDeviceToken(Guid accountId);
        Task<bool> SendNotification(ICollection<Guid> accountIds, CreateNotificationModel model);
        Task<NotificationViewModel> UpdateNotification(Guid id, UpdateNotificationModel model);
        Task<bool> MakeAsRead(Guid accountId);
        Task<bool> DeleteNotification(Guid id);
    }
}
