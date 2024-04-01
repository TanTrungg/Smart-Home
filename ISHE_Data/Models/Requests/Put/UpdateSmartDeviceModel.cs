namespace ISHE_Data.Models.Requests.Put
{
    public class UpdateSmartDeviceModel
    {
        public Guid? ManufacturerId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? DeviceType { get; set; }
        public int? Price { get; set; }
        public int? InstallationPrice { get; set; }
        public string? Status { get; set; }
    }
}
