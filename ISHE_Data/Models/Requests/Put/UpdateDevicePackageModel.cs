using ISHE_Data.Models.Requests.Post;
using Microsoft.AspNetCore.Http;

namespace ISHE_Data.Models.Requests.Put
{
    public class UpdateDevicePackageModel
    {
        public Guid? ManufacturerId { get; set; }
        
        public string? Name { get; set; }
        public int? WarrantyDuration { get; set; }
        public int? CompletionTime { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public IFormFile? Image { get; set; }
        public List<Guid>? PromotionIds { get; set; }
        public List<SmartDevices>? SmartDevices { get; set; } = new List<SmartDevices>();


    }
}
