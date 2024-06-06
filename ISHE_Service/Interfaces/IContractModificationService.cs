using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Get;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;

namespace ISHE_Service.Interfaces
{
    public interface IContractModificationService
    {
        Task<ListViewModel<ContractModificationViewModel>> GetContractModifications(ContractModificationFilterModel filter, PaginationRequestModel pagination);
        Task<ContractModificationViewModel> GetContractModification(Guid id);
        Task<ContractModificationViewModel> CreateContractModification(CreateContractModificationModel model);
        Task<ContractModificationViewModel> UpdateContractModification(Guid id, UpdateContractModificationModel model);
    }
}
