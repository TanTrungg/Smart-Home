namespace ISHE_Data.Models.Requests.Post
{
    public class CreateSurveyRequestModel
    {
        public Guid CustomerId { get; set; }
        public string SurveyDate { get; set; } = null!;
        public string Description { get; set; } = null!;

    }
}
