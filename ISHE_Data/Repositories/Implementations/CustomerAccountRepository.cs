using ISHE_Data.Entities;
using ISHE_Data.Repositories.Interfaces;

namespace ISHE_Data.Repositories.Implementations
{
    public class CustomerAccountRepository : Repository<CustomerAccount>, ICustomerAccountRepository
    {
        public CustomerAccountRepository(SMART_HOME_DBContext context) : base(context)
        {
        }
    }
}
