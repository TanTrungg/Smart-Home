using ISHE_Data.Entities;
using ISHE_Data.Repositories.Interfaces;

namespace ISHE_Data.Repositories.Implementations
{
    public class StaffAccountRepository : Repository<StaffAccount>, IStaffAccountRepository
    {
        public StaffAccountRepository(SMART_HOME_DBContext context) : base(context)
        {
        }
    }
}
