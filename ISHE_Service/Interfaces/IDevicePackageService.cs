using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Get;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;

namespace ISHE_Service.Interfaces
{
    public interface IDevicePackageService
    {
        Task<ListViewModel<DevicePackageViewModel>> GetDevicePackages(DevicePackageFilterModel filter, PaginationRequestModel pagination);
        Task<DevicePackageDetailViewModel> GetDevicePackage(Guid id);
        Task<DevicePackageDetailViewModel> CreateDevicePackage(CreateDevicePackageModel model);
        Task<DevicePackageDetailViewModel> UpdateDevicePackage(Guid id, UpdateDevicePackageModel model);
    }
}
