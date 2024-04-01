using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class SmartDevice
    {
        public SmartDevice()
        {
            ContractDetails = new HashSet<ContractDetail>();
            Images = new HashSet<Image>();
            SmartDevicePackages = new HashSet<SmartDevicePackage>();
        }

        public Guid Id { get; set; }
        public Guid ManufacturerId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Price { get; set; }
        public int InstallationPrice { get; set; }
        public string? DeviceType { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual Manufacturer Manufacturer { get; set; } = null!;
        public virtual ICollection<ContractDetail> ContractDetails { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<SmartDevicePackage> SmartDevicePackages { get; set; }
    }
}
