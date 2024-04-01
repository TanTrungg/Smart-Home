using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class Image
    {
        public Guid Id { get; set; }
        public Guid? DevicePackageId { get; set; }
        public Guid? SmartDeviceId { get; set; }
        public string Url { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual DevicePackage? DevicePackage { get; set; }
        public virtual SmartDevice? SmartDevice { get; set; }
    }
}
