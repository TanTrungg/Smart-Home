namespace ISHE_Data.Models.Requests.Put
{
    public class UpdatePromotionModel
    {
        public string? Name { get; set; }
        public int? DiscountAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; } = null!;
        public List<Guid>? DevicePackageIds { get; set; } = new List<Guid>();

    }
}
