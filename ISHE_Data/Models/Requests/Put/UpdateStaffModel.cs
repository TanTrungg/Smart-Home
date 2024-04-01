namespace ISHE_Data.Models.Requests.Put
{
    public class UpdateStaffModel
    {
        public string? FullName { get; set; }
        public string? Status { get; set; }
        public bool? IsLead { get; set; }
        public Guid? StaffLeadId { get; set; }


        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
