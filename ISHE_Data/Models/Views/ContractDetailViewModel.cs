namespace ISHE_Data.Models.Views
{
    public class ContractDetailViewModel
    {
        public Guid SmartDeviceId { get; set; }
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int Price { get; set; }
        public int Quantity { get; set; }
        //public bool IsInstallation { get; set; }
        public int InstallationPrice { get; set; }
        public string Manufacturer { get; set; } = null!;

        public string Image { get; set; } = null!;

        public DateTime CreateAt { get; set; }
    }
}
