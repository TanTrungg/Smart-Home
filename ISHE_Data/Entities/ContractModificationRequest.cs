using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class ContractModificationRequest
    {
        public Guid Id { get; set; }
        public string ContractId { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual Contract Contract { get; set; } = null!;
    }
}
