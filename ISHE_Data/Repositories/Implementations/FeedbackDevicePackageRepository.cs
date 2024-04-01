using ISHE_Data.Entities;
using ISHE_Data.Repositories.Interfaces;

namespace ISHE_Data.Repositories.Implementations
{
    public class FeedbackDevicePackageRepository : Repository<FeedbackDevicePackage>, IFeedbackDevicePackageRepository
    {
        public FeedbackDevicePackageRepository(SMART_HOME_DBContext context) : base(context)
        {
        }
    }
}
