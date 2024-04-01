using ISHE_Utility.Enum;

namespace ISHE_Data.Models.Requests.Filters
{
    public class ContractFilterModel
    {
        public Guid? StaffId { get; set; }
        public Guid? CustomerId { get; set; }
        public ContractStatus? Status { get; set; }
    }
}
