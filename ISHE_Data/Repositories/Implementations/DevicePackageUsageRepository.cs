using ISHE_Data.Entities;
using ISHE_Data.Repositories.Interfaces;

namespace ISHE_Data.Repositories.Implementations
{
    public class DevicePackageUsageRepository : Repository<DevicePackageUsage>, IDevicePackageUsageRepository
    {
        public DevicePackageUsageRepository(SMART_HOME_DBContext context) : base(context)
        {
        }
    }
}
