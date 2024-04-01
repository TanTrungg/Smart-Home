namespace ISHE_Data.Models.Requests.Post
{
    public class CreateContractModificationModel
    {
        public string ContractId { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Description { get; set; } = null!;

    }
}
