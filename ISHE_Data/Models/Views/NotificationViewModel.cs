namespace ISHE_Data.Models.Views
{
    public class NotificationViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Body { get; set; } = null!;

        public NotificationDataViewModel Data { get; set; } = null!;
    }
}
