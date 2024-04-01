namespace ISHE_Data.Models.Views
{
    public class SmartDeviceDetailViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int? Price { get; set; }
        public int InstallationPrice { get; set; }

        public string? DeviceType { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual ManufacturerViewModel Manufacturer { get; set; } = null!;
        public virtual ICollection<ImageViewModel> Images { get; set; } = new List<ImageViewModel>();
    }
}
