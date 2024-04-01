using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;

namespace ISHE_Service.Interfaces
{
    public interface IContractModificationService
    {
        Task<ContractModificationViewModel> GetContractModification(Guid id);
        Task<ContractModificationViewModel> CreateContractModification(CreateContractModificationModel model);
        Task<ContractModificationViewModel> UpdateContractModification(Guid id, UpdateContractModificationModel model);
    }
}
