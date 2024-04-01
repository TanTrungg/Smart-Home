namespace ISHE_Data.Models.Requests.Put
{
    public class UpdateSurveyRequestModel
    {
        public string? SurveyDate { get; set; }
        public Guid? StaffId { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
    }
}
