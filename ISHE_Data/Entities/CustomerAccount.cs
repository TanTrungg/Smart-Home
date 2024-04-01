using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class CustomerAccount
    {
        public CustomerAccount()
        {
            Contracts = new HashSet<Contract>();
            FeedbackDevicePackages = new HashSet<FeedbackDevicePackage>();
            SurveyRequests = new HashSet<SurveyRequest>();
        }

        public Guid AccountId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public string Address { get; set; } = null!;
        public string? Otp { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<FeedbackDevicePackage> FeedbackDevicePackages { get; set; }
        public virtual ICollection<SurveyRequest> SurveyRequests { get; set; }
    }
}
