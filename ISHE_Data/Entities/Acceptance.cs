using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class Acceptance
    {
        public Guid Id { get; set; }
        public string ContractId { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual Contract Contract { get; set; } = null!;
    }
}
