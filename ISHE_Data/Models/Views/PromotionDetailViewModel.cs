namespace ISHE_Data.Models.Views
{
    public class PromotionDetailViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int? DiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual ICollection<PatialDevicePackageViewModel> DevicePackages { get; set; } = new List<PatialDevicePackageViewModel>();
    }

    public class PatialDevicePackageViewModel
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
        //public virtual ICollection<PromotionViewModel>? Promotions { get; set; }
        //public virtual ICollection<ConstructionContract> ConstructionContracts { get; set; }
        // public virtual ICollection<FeedbackDevicePackage> FeedbackDevicePackages { get; set; }
        public virtual string Images { get; set; } = null!;
    }
}
