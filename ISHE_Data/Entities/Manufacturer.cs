using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class Manufacturer
    {
        public Manufacturer()
        {
            DevicePackages = new HashSet<DevicePackage>();
            SmartDevices = new HashSet<SmartDevice>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreateAt { get; set; }
        public string? Image { get; set; }
        public string? Origin { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<DevicePackage> DevicePackages { get; set; }
        public virtual ICollection<SmartDevice> SmartDevices { get; set; }
    }
}
