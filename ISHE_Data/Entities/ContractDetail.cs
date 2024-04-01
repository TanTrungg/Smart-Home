using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class ContractDetail
    {
        public Guid Id { get; set; }
        public string ContractId { get; set; } = null!;
        public Guid SmartDeviceId { get; set; }
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool IsInstallation { get; set; }
        public int InstallationPrice { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual Contract Contract { get; set; } = null!;
        public virtual SmartDevice SmartDevice { get; set; } = null!;
    }
}
