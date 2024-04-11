namespace ISHE_Data.Models.Views
{
    public class StaffGroupViewModel
    {
        public Guid LeadAccountId { get; set; }
        public string LeadPhoneNumber { get; set; } = null!;
        public string LeadFullName { get; set; } = null!;
        public string LeadEmail { get; set; } = null!;
        public string LeadAvatar { get; set; } = null!;
        public bool LeadIsLead { get; set; }
        public List<StaffViewModel> StaffMembers { get; set; } = new List<StaffViewModel>();
    }
}
