namespace ISHE_Data.Models.Requests.Post
{
    public class CreatePromotionModel
    {
        public string Name { get; set; } = null!;
        public int? DiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
        public List<Guid>? DevicePackageIds { get; set; }
    }
}
