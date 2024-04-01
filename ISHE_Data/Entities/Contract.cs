using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class Contract
    {
        public Contract()
        {
            ContractDetails = new HashSet<ContractDetail>();
            ContractModificationRequests = new HashSet<ContractModificationRequest>();
            DevicePackageUsages = new HashSet<DevicePackageUsage>();
            Payments = new HashSet<Payment>();
        }

        public string Id { get; set; } = null!;
        public Guid SurveyId { get; set; }
        public Guid StaffId { get; set; }
        public Guid TellerId { get; set; }
        public Guid CustomerId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime StartPlanDate { get; set; }
        public DateTime EndPlanDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public int TotalAmount { get; set; }
        public string? ImageUrl { get; set; }
        public int Deposit { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual CustomerAccount Customer { get; set; } = null!;
        public virtual StaffAccount Staff { get; set; } = null!;
        public virtual Survey Survey { get; set; } = null!;
        public virtual TellerAccount Teller { get; set; } = null!;
        public virtual Acceptance? Acceptance { get; set; }
        public virtual ICollection<ContractDetail> ContractDetails { get; set; }
        public virtual ICollection<ContractModificationRequest> ContractModificationRequests { get; set; }
        public virtual ICollection<DevicePackageUsage> DevicePackageUsages { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
