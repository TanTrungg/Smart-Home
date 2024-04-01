using ISHE_Data.Entities;
using ISHE_Data.Repositories.Interfaces;

namespace ISHE_Data.Repositories.Implementations
{
    public class SmartDevicePackageRepository : Repository<SmartDevicePackage>, ISmartDevicePackageRepository
    {
        public SmartDevicePackageRepository(SMART_HOME_DBContext context) : base(context)
        {
        }
    }
}
