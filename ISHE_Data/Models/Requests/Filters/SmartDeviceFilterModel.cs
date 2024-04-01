using ISHE_Utility.Enum;

namespace ISHE_Data.Models.Requests.Filters
{
    public class SmartDeviceFilterModel
    {
        public string? Name { get; set; }
        public string? DeviceType { get; set; }

        public string? ManufacturerName { get; set; }
        public Guid? ManufacturerId { get; set; }

        public SmartDeviceStatus? Status { get; set; }


        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }

    }
}
