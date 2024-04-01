using ISHE_Data.Entities;
using ISHE_Data.Repositories.Interfaces;

namespace ISHE_Data.Repositories.Implementations
{
    public class DeviceTokenRepository : Repository<DeviceToken>, IDeviceTokenRepository
    {
        public DeviceTokenRepository(SMART_HOME_DBContext context) : base(context)
        {
        }
    }
}
