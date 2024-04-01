using ISHE_Utility.Enum;

namespace ISHE_Data.Models.Requests.Filters
{
    public class SurveyFilterModel
    {
        public DateTime? AppointmentDate { get; set; }
        public SurveyStatus? Status { get; set; }
    }
}
