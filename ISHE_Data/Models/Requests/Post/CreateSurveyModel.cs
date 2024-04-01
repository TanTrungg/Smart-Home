namespace ISHE_Data.Models.Requests.Post
{
    public class CreateSurveyModel
    {
        public Guid SurveyRequestId { get; set; }
        public Guid? RecommendDevicePackageId { get; set; }
        //public Guid StaffId { get; set; }
        public decimal? RoomArea { get; set; }
        public string Description { get; set; } = null!;
        public DateTime AppointmentDate { get; set; }

    }
}
