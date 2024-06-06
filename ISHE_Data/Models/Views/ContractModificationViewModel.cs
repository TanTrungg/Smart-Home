namespace ISHE_Data.Models.Views
{
    public class ContractModificationViewModel
    {
        public Guid Id { get; set; }
        public string ContractId { get; set; } = null!;
        public Guid CustomerId { get; set; }
        public string Type { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        //public PartialCustomerViewModel Customer { get; set; } = null!;
        public DateTime CreateAt { get; set; }
    }
}
