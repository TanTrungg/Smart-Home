namespace ISHE_Service.Interfaces
{
    public interface ISendMailService
    {
        Task SendEmail(string userEmail, string title, string message);
    }
}
