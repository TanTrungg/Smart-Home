using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class OwnerAccount
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Avatar { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
