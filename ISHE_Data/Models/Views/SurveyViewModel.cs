namespace ISHE_Data.Models.Views
{
    public class SurveyViewModel
    {
        public Guid Id { get; set; }
        public decimal? RoomArea { get; set; }
        public string Description { get; set; } = null!;
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual DevicePackageDetailViewModel? RecommendDevicePackage { get; set; }
        public virtual SurveyRequestViewModel SurveyRequest { get; set; } = null!;
        //public virtual Contract? Contract { get; set; }
    }

    public class PartialSurveyViewModel
    {
        public Guid Id { get; set; }
        public decimal? RoomArea { get; set; }
        public string Description { get; set; } = null!;
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }
    }
}
