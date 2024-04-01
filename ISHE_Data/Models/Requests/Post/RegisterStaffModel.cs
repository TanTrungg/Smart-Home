namespace ISHE_Data.Models.Requests.Post
{
    public class RegisterStaffModel
    {
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsLead { get; set; }
        public Guid? StaffLeadId { get; set; }

    }
}
