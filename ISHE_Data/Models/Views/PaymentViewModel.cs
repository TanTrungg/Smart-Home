namespace ISHE_Data.Models.Views
{
    public class PaymentViewModel
    {
        public string Id { get; set; } = null!;
        public string ContractId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public int Amount { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        //public virtual Contract Contract { get; set; } = null!;
    }
}
