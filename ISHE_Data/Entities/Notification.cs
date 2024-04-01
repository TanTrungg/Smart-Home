using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class Notification
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public string? Type { get; set; }
        public string? Link { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
