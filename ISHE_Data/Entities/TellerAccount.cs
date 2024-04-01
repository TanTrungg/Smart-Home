using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class TellerAccount
    {
        public TellerAccount()
        {
            Contracts = new HashSet<Contract>();
        }

        public Guid AccountId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Avatar { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
