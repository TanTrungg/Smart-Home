using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Views;

namespace ISHE_Service.Interfaces
{
    public interface IPaymentService
    {
        Task ProcessCashPayment(CreatePaymentModel model);
        Task<dynamic> ProcessZalopayPayment(CreatePaymentModel model);
        Task<dynamic> IsValidCallback(dynamic cbdata);
        Task<List<PaymentViewModel>> GetRevenues(int? year);
    }
}
