using System;
using System.Collections.Generic;

namespace ISHE_Data.Entities
{
    public partial class SurveyRequest
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? StaffId { get; set; }
        public DateTime SurveyDate { get; set; }
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual CustomerAccount Customer { get; set; } = null!;
        public virtual StaffAccount? Staff { get; set; }
        public virtual Survey? Survey { get; set; }
    }
}
