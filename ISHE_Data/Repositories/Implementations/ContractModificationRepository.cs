using ISHE_Data.Entities;
using ISHE_Data.Repositories.Interfaces;

namespace ISHE_Data.Repositories.Implementations
{
    public class ContractModificationRepository : Repository<ContractModificationRequest>, IContractModificationRepository
    {
        public ContractModificationRepository(SMART_HOME_DBContext context) : base(context)
        {
        }
    }
}
