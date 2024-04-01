using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISHE_Data.Models.Requests.Put
{
    public class UpdateTellerModel
    {
        public string? FullName { get; set; }
        public string? Status { get; set; }

        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
