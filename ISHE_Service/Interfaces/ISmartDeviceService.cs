using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Get;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;

namespace ISHE_Service.Interfaces
{
    public interface ISmartDeviceService
    {
        Task<ListViewModel<SmartDeviceDetailViewModel>> GetSmartDevices(SmartDeviceFilterModel filter, PaginationRequestModel pagination);
        Task<SmartDeviceDetailViewModel> GetSmartDevice(Guid id);
        Task<SmartDeviceDetailViewModel> CreateSmartDevice(CreateSmartDeviceModel model);
        Task<SmartDeviceDetailViewModel> UpdateSmartDevice(Guid id, UpdateSmartDeviceModel model);
        Task<SmartDeviceDetailViewModel> UpdateSmartDeviceImage(Guid id, UpdateImageModel model);
        Task RemoveSmartDeviceImage(List<Guid> imageIds);
    }
}
