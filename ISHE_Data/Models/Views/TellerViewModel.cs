using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISHE_Data.Models.Views
{
    public class TellerViewModel
    {
        public Guid AccountId { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string RoleName { get; set; } = null!;

        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }
    }
}
