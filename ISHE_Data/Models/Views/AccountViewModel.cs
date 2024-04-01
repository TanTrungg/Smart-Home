namespace ISHE_Data.Models.Views
{
    public class AccountViewModel
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public string RoleName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

    }
}
