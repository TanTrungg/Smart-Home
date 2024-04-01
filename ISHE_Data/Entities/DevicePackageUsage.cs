using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class DevicePackageUsage
    {
        public Guid Id { get; set; }
        public string ContractId { get; set; } = null!;
        public Guid DevicePackageId { get; set; }
        public int? DiscountAmount { get; set; }
        public int Price { get; set; }
        public int? WarrantyDuration { get; set; }
        public DateTime? StartWarranty { get; set; }
        public DateTime? EndWarranty { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual Contract Contract { get; set; } = null!;
        public virtual DevicePackage DevicePackage { get; set; } = null!;
    }
}
