using ISHE_Utility.Enum;

namespace ISHE_Data.Models.Requests.Filters
{
    public class SurveyRequestFilterModel
    {
        public Guid? CustomerId { get; set; }
        public Guid? StaffId { get; set; }

        public DateTime? SurveyDate { get; set; }
        public SurveyRequestStatus? Status { get; set; }

    }
}
