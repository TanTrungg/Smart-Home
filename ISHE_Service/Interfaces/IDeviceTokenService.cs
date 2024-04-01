using ISHE_Data.Models.Requests.Post;

namespace ISHE_Service.Interfaces
{
    public interface IDeviceTokenService
    {
        Task<bool> CreateDeviceToken(Guid accountId, CreateDeviceTokenModel model);
    }
}
