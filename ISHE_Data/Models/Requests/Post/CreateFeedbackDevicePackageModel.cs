namespace ISHE_Data.Models.Requests.Post
{
    public class CreateFeedbackDevicePackageModel
    {
        public Guid CustomerId { get; set; }
        public Guid DevicePackageId { get; set; }
        public int Rating { get; set; }
        public string? Content { get; set; }
    }
}
