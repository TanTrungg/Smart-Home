using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Get;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;
using Microsoft.AspNetCore.Http;

namespace ISHE_Service.Interfaces
{
    public interface IContractService
    {
        Task<ListViewModel<PartialContractViewModel>> GetContracts(ContractFilterModel filter, PaginationRequestModel pagination);
        Task<ContractViewModel> GetContract(string id);
        Task<ContractViewModel> CreateContract(CreateContractModel model);
        Task<ContractViewModel> UpdateContract(string id, UpdateContractModel model);
        Task<ContractViewModel> UploadContractImage(string contractId, IFormFile image);
        Task<ContractViewModel> UploadContractAcceptance(string contractId, IFormFile image);
    }
}
