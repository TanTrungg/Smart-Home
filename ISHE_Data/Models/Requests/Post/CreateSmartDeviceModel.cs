using Microsoft.AspNetCore.Http;

namespace ISHE_Data.Models.Requests.Post
{
    public class CreateSmartDeviceModel
    {
        public Guid ManufacturerId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? DeviceType { get; set; }
        public int Price { get; set; }
        public int InstallationPrice { get; set; }
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}
