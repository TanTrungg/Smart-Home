using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class StaffAccount
    {
        public StaffAccount()
        {
            Contracts = new HashSet<Contract>();
            InverseStaffLead = new HashSet<StaffAccount>();
            SurveyRequests = new HashSet<SurveyRequest>();
        }

        public Guid AccountId { get; set; }
        public Guid? StaffLeadId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public bool IsLead { get; set; }
        public string? Avatar { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual StaffAccount? StaffLead { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<StaffAccount> InverseStaffLead { get; set; }
        public virtual ICollection<SurveyRequest> SurveyRequests { get; set; }
    }
}
