using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class DevicePackage
    {
        public DevicePackage()
        {
            DevicePackageUsages = new HashSet<DevicePackageUsage>();
            FeedbackDevicePackages = new HashSet<FeedbackDevicePackage>();
            Images = new HashSet<Image>();
            SmartDevicePackages = new HashSet<SmartDevicePackage>();
            Surveys = new HashSet<Survey>();
        }

        public Guid Id { get; set; }
        public Guid ManufacturerId { get; set; }
        public Guid? PromotionId { get; set; }
        public string Name { get; set; } = null!;
        public int? WarrantyDuration { get; set; }
        public string Description { get; set; } = null!;
        public int Price { get; set; }
        public int CompletionTime { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual Manufacturer Manufacturer { get; set; } = null!;
        public virtual Promotion? Promotion { get; set; }
        public virtual ICollection<DevicePackageUsage> DevicePackageUsages { get; set; }
        public virtual ICollection<FeedbackDevicePackage> FeedbackDevicePackages { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<SmartDevicePackage> SmartDevicePackages { get; set; }
        public virtual ICollection<Survey> Surveys { get; set; }
    }
}
