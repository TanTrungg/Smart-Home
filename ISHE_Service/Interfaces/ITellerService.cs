using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Get;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISHE_Service.Interfaces
{
    public interface ITellerService
    {
        Task<ListViewModel<TellerViewModel>> GetTellers(TellerFilterModel filter, PaginationRequestModel pagination);
        Task<TellerViewModel> GetTeller(Guid id);
        Task<TellerViewModel> CreateTeller(RegisterTellerModel model);
        Task<TellerViewModel> UpdateTeller(Guid id, UpdateTellerModel model);
        Task<TellerViewModel> UploadAvatar(Guid id, IFormFile image);
    }
}
