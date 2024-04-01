using ISHE_Data.Models.Internal;
using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Get;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Views;

namespace ISHE_Service.Interfaces
{
    public interface IAccountService
    {
        Task<AuthViewModel> Authenticated(AuthRequest auth);
        Task<AuthViewModel> RefreshAuthentication(string currentToken);
        Task<AuthModel> GetAuth(Guid id);

        //--
        Task<ListViewModel<AccountViewModel>> GetAccounts(AccountFilterModel filter, PaginationRequestModel pagination);
        Task<AccountViewModel> GetAccount(Guid id);
        Task<Guid> CreateAccount(string phoneNumber, string password, string role);

    }
}
