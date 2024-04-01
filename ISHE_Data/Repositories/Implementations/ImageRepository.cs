using ISHE_Data.Entities;
using ISHE_Data.Repositories.Interfaces;

namespace ISHE_Data.Repositories.Implementations
{
    public class ImageRepository : Repository<Image>, IImageRepository
    {
        public ImageRepository(SMART_HOME_DBContext context) : base(context)
        {
        }
    }
}
