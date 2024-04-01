using ISHE_Utility.Enum;

namespace ISHE_Data.Models.Requests.Filters
{
    public class AccountFilterModel
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public AccountStatus? Status { get; set; }
    }
}
