using ISHE_Data.Entities;
using ISHE_Data.Repositories.Interfaces;

namespace ISHE_Data.Repositories.Implementations
{
    public class OwnerAccountRepository : Repository<OwnerAccount>, IOwnerAccountRepository
    {
        public OwnerAccountRepository(SMART_HOME_DBContext context) : base(context)
        {
        }
    }
}
