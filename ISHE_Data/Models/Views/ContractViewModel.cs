using ISHE_Data.Entities;

namespace ISHE_Data.Models.Views
{
    public class ContractViewModel
    {
        public string Id { get; set; } = null!;
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

        public virtual PartialCustomerViewModel Customer { get; set; } = null!;
        public virtual StaffViewModel Staff { get; set; } = null!;
        public virtual PartialSurveyViewModel Survey { get; set; } = null!;
        public virtual TellerViewModel Teller { get; set; } = null!;
        public virtual AcceptanceViewModel? Acceptance { get; set; }
        public virtual ICollection<DevicePackageUsageViewModel> DevicePackageUsages { get; set; } = new List<DevicePackageUsageViewModel>();
        public virtual ICollection<ContractDetailViewModel> ContractDetails { get; set; } = new List<ContractDetailViewModel>();
        public virtual ICollection<ContractModificationViewModel> ContractModificationRequests { get; set; } = new List<ContractModificationViewModel>();
        public virtual ICollection<PaymentViewModel> Payments { get; set; } = new List<PaymentViewModel>();
    }
}
