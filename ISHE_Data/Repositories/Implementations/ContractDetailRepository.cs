using ISHE_Data.Entities;
using ISHE_Data.Repositories.Interfaces;

namespace ISHE_Data.Repositories.Implementations
{
    public class ContractDetailRepository : Repository<ContractDetail>, IContractDetailRepository
    {
        public ContractDetailRepository(SMART_HOME_DBContext context) : base(context)
        {
        }
    }
}
