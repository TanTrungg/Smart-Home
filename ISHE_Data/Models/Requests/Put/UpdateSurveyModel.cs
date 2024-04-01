namespace ISHE_Data.Models.Requests.Put
{
    public class UpdateSurveyModel
    {
        public Guid? RecommendDevicePackageId { get; set; }
        //public Guid? StaffId { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public decimal? RoomArea { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }

    }
}
