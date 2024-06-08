namespace ISHE_Data.Models.Requests.Post
{
    public class CreateMailInfoRequest
    {
        public string Email { get; set; } = null!;
        public string Title { get; set; } = null!;

        public string Message { get; set; } = null!;
    }
}
