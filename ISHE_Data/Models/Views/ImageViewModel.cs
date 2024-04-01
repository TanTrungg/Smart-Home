namespace ISHE_Data.Models.Views
{
    public class ImageViewModel
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = null!;
        public DateTime CreateAt { get; set; }

    }
}
