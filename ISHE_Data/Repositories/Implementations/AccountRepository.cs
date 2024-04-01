using ISHE_Data.Entities;
using ISHE_Data.Repositories.Interfaces;

namespace ISHE_Data.Repositories.Implementations
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(SMART_HOME_DBContext context) : base(context)
        {
        }
    }
}
