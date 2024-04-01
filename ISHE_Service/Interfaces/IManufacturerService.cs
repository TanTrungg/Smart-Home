using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Views;

namespace ISHE_Service.Interfaces
{
    public interface IManufacturerService
    {
        Task<List<ManufacturerViewModel>> GetManufacturers(ManufacturerFilterModel filter);
        Task<ManufacturerViewModel> GetManufacturer(Guid id);
        Task<ManufacturerViewModel> CreateManufacturer(CreateManufacturerModel model);
        Task<ManufacturerViewModel> UpdateManufacturer(Guid id, CreateManufacturerModel model);
    }
}
