namespace ISHE_Data.Models.Views
{
    public class MostDevicePackageViewModel
    {
        public int TotalSold {  get; set; }
        public DevicePackageViewModel DevicePackage { get; set; } = null!;
    }
}
