using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class DeviceToken
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string? Token { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
