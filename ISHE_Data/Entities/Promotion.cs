using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class Promotion
    {
        public Promotion()
        {
            DevicePackages = new HashSet<DevicePackage>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int? DiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual ICollection<DevicePackage> DevicePackages { get; set; }
    }
}
