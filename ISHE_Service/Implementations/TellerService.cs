using AutoMapper;
using AutoMapper.QueryableExtensions;
using ISHE_Data;
using ISHE_Data.Entities;
using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Get;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;
using ISHE_Data.Repositories.Interfaces;
using ISHE_Service.Interfaces;
using ISHE_Utility.Constants;
using ISHE_Utility.Exceptions;
using ISHE_Utility.Helpers.PasswordHasher;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISHE_Service.Implementations
{
    public class TellerService : BaseService, ITellerService
    {
        private readonly ITellerAccountRepository _tellerRepository;

        private readonly IAccountService _accountService;
        private readonly ICloudStorageService _cloudStorageService;
        public TellerService(IUnitOfWork unitOfWork, IMapper mapper, IAccountService accountService, ICloudStorageService cloudStorageService) : base(unitOfWork, mapper)
        {
            _tellerRepository = unitOfWork.TellerAccount;
            _accountService = accountService;
            _cloudStorageService = cloudStorageService;
        }

        public async Task<TellerViewModel> CreateTeller(RegisterTellerModel model)
        {
            var result = 0;
            var accountId = Guid.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    accountId = await _accountService.CreateAccount(model.PhoneNumber, model.Password, AccountRole.Teller);

                    var staff = new TellerAccount
                    {
                        AccountId = accountId,
                        FullName = model.FullName,
                        Email = model.Email,
                    };

                    _tellerRepository.Add(staff);

                    result = await _unitOfWork.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };
            return result > 0 ? await GetTeller(accountId) : null!;
        }

        public async Task<TellerViewModel> GetTeller(Guid id)
        {
            return await _tellerRepository.GetMany(staff => staff.AccountId.Equals(id))
               .ProjectTo<TellerViewModel>(_mapper.ConfigurationProvider)
               .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy tellers.");
        }

        public async Task<ListViewModel<TellerViewModel>> GetTellers(TellerFilterModel filter, PaginationRequestModel pagination)
        {
            var query = _tellerRepository.GetAll();

            if (!string.IsNullOrEmpty(filter.FullName))
            {
                query = query.Where(teller => teller.FullName.Contains(filter.FullName));
            }

            if (!string.IsNullOrEmpty(filter.PhoneNumber))
            {
                query = query.Where(teller => teller.Account.PhoneNumber.Contains(filter.PhoneNumber));
            }

            if (!string.IsNullOrEmpty(filter.Status.ToString()))
            {
                query = query.Where(teller => teller.Account.Status.Equals(filter.Status.ToString()));
            }

            var totalRow = await query.AsNoTracking().CountAsync();
            var paginatedQuery = query
                .OrderByDescending(teller => teller.Account.CreateAt)
                .Skip(pagination.PageNumber * pagination.PageSize)
                .Take(pagination.PageSize);
            var tellers = await paginatedQuery
                .ProjectTo<TellerViewModel>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
            return new ListViewModel<TellerViewModel>
            {
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalRow = totalRow
                },
                Data = tellers
            };
        }

        public async Task<TellerViewModel> UpdateTeller(Guid id, UpdateTellerModel model)
        {
            var teller = await _tellerRepository.GetMany(teller => teller.AccountId.Equals(id))
                                               .Include(teller => teller.Account)
                                               .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy teller");

            teller.FullName = model.FullName ?? teller.FullName;
            teller.Account.Status = model.Status ?? teller.Account.Status;

            if (!string.IsNullOrEmpty(model.OldPassword))
            {
                if (!PasswordHasher.VerifyPassword(model.OldPassword, teller.Account.PasswordHash))
                {
                    throw new BadRequestException("Mật khẩu cũ không chính sát.");
                }
                if (model.NewPassword != null)
                {
                    teller.Account.PasswordHash = PasswordHasher.HashPassword(model.NewPassword);
                }
            }
            _tellerRepository.Update(teller);

            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetTeller(teller.AccountId) : null!;
        }

        public async Task<TellerViewModel> UploadAvatar(Guid id, IFormFile image)
        {
            if (!image.ContentType.StartsWith("image/"))
            {
                throw new BadRequestException("File không phải là hình ảnh");
            }
            var teller = await _tellerRepository.GetMany(teller => teller.AccountId.Equals(id)).FirstOrDefaultAsync();
            if (teller != null)
            {
                //xóa hình cũ trong firebase
                if (!string.IsNullOrEmpty(teller.Avatar))
                {
                    await _cloudStorageService.Delete(id);
                }

                //upload hình mới
                var url = await _cloudStorageService.Upload(id, image.ContentType, image.OpenReadStream());

                teller.Avatar = url;

                _tellerRepository.Update(teller);
            }
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetTeller(id) : null!;
        }
    }
}
