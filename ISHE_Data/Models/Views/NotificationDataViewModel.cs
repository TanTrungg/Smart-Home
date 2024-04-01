namespace ISHE_Data.Models.Views
{
    public class NotificationDataViewModel
    {
        public string? Link { get; set; }

        public string? Type { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreateAt { get; set; }
    }
}
