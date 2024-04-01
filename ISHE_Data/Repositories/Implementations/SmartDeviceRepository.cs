using ISHE_Data.Entities;
using ISHE_Data.Repositories.Interfaces;

namespace ISHE_Data.Repositories.Implementations
{
    public class SmartDeviceRepository : Repository<SmartDevice>, ISmartDeviceRepository
    {
        public SmartDeviceRepository(SMART_HOME_DBContext context) : base(context)
        {
        }
    }
}
