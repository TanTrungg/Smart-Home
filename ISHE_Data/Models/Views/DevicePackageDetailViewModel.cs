using ISHE_Data.Entities;

namespace ISHE_Data.Models.Views
{
    public class DevicePackageDetailViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int? WarrantyDuration { get; set; }
        public int CompletionTime { get; set; }

        public string Description { get; set; } = null!;
        public int Price { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual ManufacturerViewModel Manufacturer { get; set; } = null!;
        //public virtual PromotionViewModel? Promotion { get; set; }
        public virtual ICollection<PromotionViewModel>? Promotions { get; set; }

        public virtual ICollection<FeedbackDevicePackageViewModel> FeedbackDevicePackages { get; set; } = new List<FeedbackDevicePackageViewModel>();
        public virtual ICollection<ImageViewModel> Images { get; set; } = new List<ImageViewModel>();
        //public virtual ICollection<Survey> Surveys { get; set; }
        public virtual ICollection<SmartDevicePackageViewModel> SmartDevicePackages { get; set; } = new List<SmartDevicePackageViewModel>();
    }
}
