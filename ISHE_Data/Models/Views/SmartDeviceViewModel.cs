namespace ISHE_Data.Models.Views
{
    public class SmartDeviceViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int? Price { get; set; }
        public int InstallationPrice { get; set; }
        public string? DeviceType { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }
        public string Manufacturer { get; set; } = null!;
        public string Image { get; set; } = null!;

    }
}
