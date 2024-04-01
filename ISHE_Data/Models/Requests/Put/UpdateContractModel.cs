using ISHE_Data.Models.Requests.Post;
using Microsoft.AspNetCore.Http;

namespace ISHE_Data.Models.Requests.Put
{
    public class UpdateContractModel
    {
        public Guid? StaffId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public string? Status { get; set; }
        public List<Guid>? DevicePackages { get; set; } = new List<Guid>();

        public List<SmartDevices>? ContractDetails { get; set; }
    }
}
