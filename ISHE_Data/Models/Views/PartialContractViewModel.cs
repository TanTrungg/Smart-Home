using ISHE_Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISHE_Data.Models.Views
{
    public class PartialContractViewModel
    {
        public string Id { get; set; } = null!;
        //public Guid SurveyId { get; set; }
        //public Guid StaffId { get; set; }
        //public Guid TellerId { get; set; }
        //public Guid CustomerId { get; set; }
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

        public virtual CustomerViewModel Customer { get; set; } = null!;
        public virtual StaffViewModel Staff { get; set; } = null!;
        //public virtual SurveyViewModel Survey { get; set; } = null!;
        public virtual TellerViewModel Teller { get; set; } = null!;
        //public virtual Acceptance? Acceptance { get; set; }
        //public virtual ICollection<ContractDetail> ContractDetails { get; set; }
        //public virtual ICollection<DevicePackageUsage> DevicePackageUsages { get; set; }
        //public virtual ICollection<Payment> Payments { get; set; }
    }
}
