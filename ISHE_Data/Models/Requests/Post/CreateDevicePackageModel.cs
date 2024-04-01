using Microsoft.AspNetCore.Http;

namespace ISHE_Data.Models.Requests.Post
{
    public class CreateDevicePackageModel
    {
        public Guid ManufacturerId { get; set; }
        public Guid? PromotionId { get; set; }
        public string Name { get; set; } = null!;
        public int? WarrantyDuration { get; set; }
        public string Description { get; set; } = null!;
        public int CompletionTime { get; set; }
        public IFormFile Image { get; set; } = null!;
        public List<SmartDevices> SmartDeviceIds { get; set; } = new List<SmartDevices>();
    }

    
}
