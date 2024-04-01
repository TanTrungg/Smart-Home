namespace ISHE_Data.Models.Views
{
    public class DevicePackageUsageViewModel
    {
        public Guid DevicePackageId { get; set; }
        public string Name { get; set; } = null!;
        public int? DiscountAmount { get; set; }
        public int Price { get; set; }
        public string Manufacturer { get; set; } = null!;
        public string Image { get; set; } = null!;

        public int? WarrantyDuration { get; set; }
        public DateTime? StartWarranty { get; set; }
        public DateTime? EndWarranty { get; set; }
        public DateTime CreateAt { get; set; }

        //public virtual Contract Contract { get; set; } = null!;
        //public virtual DevicePackageViewModel DevicePackage { get; set; } = null!;
    }
}
