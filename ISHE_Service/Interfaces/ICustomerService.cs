using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Get;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;
using Microsoft.AspNetCore.Http;

namespace ISHE_Service.Interfaces
{
    public interface ICustomerService
    {
        Task<ListViewModel<CustomerViewModel>> GetCustomers(CustomerFilterModel filter, PaginationRequestModel pagination);
        Task<CustomerViewModel> GetCustomer(Guid id);
        Task<CustomerViewModel> CreateCustomer(RegisterCustomerModel model);
        Task<CustomerViewModel> UpdateCustomer(Guid id, UpdateCustomerModel model);
        Task<CustomerViewModel> UploadAvatar(Guid id, IFormFile image);
    }
}
