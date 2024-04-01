using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class SmartDevicePackage
    {
        public Guid SmartDeviceId { get; set; }
        public Guid DevicePackageId { get; set; }
        public int SmartDeviceQuantity { get; set; }

        public virtual DevicePackage DevicePackage { get; set; } = null!;
        public virtual SmartDevice SmartDevice { get; set; } = null!;
    }
}
