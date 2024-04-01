using ISHE_Utility.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISHE_Data.Models.Requests.Filters
{
    public class TellerFilterModel
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public AccountStatus? Status { get; set; }
    }
}
