using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class Account
    {
        public Account()
        {
            DeviceTokens = new HashSet<DeviceToken>();
            Notifications = new HashSet<Notification>();
        }

        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual CustomerAccount? CustomerAccount { get; set; }
        public virtual OwnerAccount? OwnerAccount { get; set; }
        public virtual StaffAccount? StaffAccount { get; set; }
        public virtual TellerAccount? TellerAccount { get; set; }
        public virtual ICollection<DeviceToken> DeviceTokens { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
