using ISHE_Utility.Helpers.ModelBinder;
using Microsoft.AspNetCore.Mvc;

namespace ISHE_Data.Models.Requests.Post
{
    [ModelBinder(BinderType = typeof(MetadataValueModelBinder))]
    public class SmartDevices
    {
        public Guid SmartDeviceId { get; set; }
        public int Quantity { get; set; }
    }
}
