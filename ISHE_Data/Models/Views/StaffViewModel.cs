namespace ISHE_Data.Models.Views
{
    public class StaffViewModel
    {
        public Guid AccountId { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string RoleName { get; set; } = null!;

        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public bool IsLead { get; set; }

        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

    }
}
