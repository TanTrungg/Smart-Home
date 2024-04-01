using ISHE_Data.Entities;
using ISHE_Data.Repositories.Interfaces;

namespace ISHE_Data.Repositories.Implementations
{
    public class TellerAccountRepository : Repository<TellerAccount>, ITellerAccountRepository
    {
        public TellerAccountRepository(SMART_HOME_DBContext context) : base(context)
        {
        }
    }
}
