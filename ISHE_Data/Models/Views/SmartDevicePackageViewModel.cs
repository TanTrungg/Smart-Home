namespace ISHE_Data.Models.Views
{
    public class SmartDevicePackageViewModel
    {
        public int SmartDeviceQuantity { get; set; }
        public virtual SmartDeviceViewModel SmartDevice { get; set; } = null!;
    }
}
