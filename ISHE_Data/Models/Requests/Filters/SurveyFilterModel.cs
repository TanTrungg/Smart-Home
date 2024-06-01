using ISHE_Utility.Enum;

namespace ISHE_Data.Models.Requests.Filters
{
    public class SurveyFilterModel
    {
        public Guid? StaffId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public SurveyStatus? Status { get; set; }
    }
}
