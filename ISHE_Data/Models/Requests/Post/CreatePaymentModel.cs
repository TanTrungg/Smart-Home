namespace ISHE_Data.Models.Requests.Post
{
    public class CreatePaymentModel
    {
        public string ContractId { get; set; } = null!;
        public string TypePayment { get; set; } = null!;
    }
}
