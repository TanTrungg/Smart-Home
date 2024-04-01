namespace ISHE_Data.Models.Views
{
    public class CustomerViewModel
    {
        public Guid AccountId { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string RoleName { get; set; } = null!;

        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public string Address { get; set; } = null!;
        public string? Otp { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

    }

    public class PartialCustomerViewModel
    {
        public Guid AccountId { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Avatar { get; set; }

    }
}
