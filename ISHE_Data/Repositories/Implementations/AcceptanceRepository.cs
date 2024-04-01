using ISHE_Data.Entities;
using ISHE_Data.Repositories.Interfaces;

namespace ISHE_Data.Repositories.Implementations
{
    public class AcceptanceRepository : Repository<Acceptance>, IAcceptanceRepository
    {
        public AcceptanceRepository(SMART_HOME_DBContext context) : base(context)
        {
        }
    }
}
