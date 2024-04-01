using ISHE_Utility.Enum;

namespace ISHE_Data.Models.Requests.Filters
{
    public class PromotionFilterModel
    {
        public string? Name { get; set; }
        public PromotionStatus? Status { get; set; }
    }
}
