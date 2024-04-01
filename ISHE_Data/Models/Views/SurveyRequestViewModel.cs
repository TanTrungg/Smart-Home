namespace ISHE_Data.Models.Views
{
    public class SurveyRequestViewModel
    {
        public Guid Id { get; set; }
        public DateTime SurveyDate { get; set; }
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }

        public virtual CustomerViewModel Customer { get; set; } = null!;
        public virtual StaffViewModel Staff { get; set; } = null!;

        //public virtual SurveyViewModel? Survey { get; set; }

    }
}
