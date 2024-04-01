using ISHE_Data.Entities;
using ISHE_Data.Repositories.Interfaces;

namespace ISHE_Data.Repositories.Implementations
{
    public class SurveyRequestRepository : Repository<SurveyRequest>, ISurveyRequestRepository
    {
        public SurveyRequestRepository(SMART_HOME_DBContext context) : base(context)
        {
        }
    }
}
