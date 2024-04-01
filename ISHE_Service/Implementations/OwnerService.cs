using AutoMapper;
using AutoMapper.QueryableExtensions;
using ISHE_Data;
using ISHE_Data.Entities;
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

namespace ISHE_Service.Implementations
{
    public class OwnerService : BaseService, IOwnerService
    {
        private readonly IOwnerAccountRepository _ownerRepository;
        private readonly IAccountService _accountService;
        private readonly ICloudStorageService _cloudStorageService;
        public OwnerService(IUnitOfWork unitOfWork, IMapper mapper, IAccountService accountService, ICloudStorageService cloudStorageService) : base(unitOfWork, mapper)
        {
            _ownerRepository = unitOfWork.OwnerAccount;
            _accountService = accountService;
            _cloudStorageService = cloudStorageService;
        }

        public async Task<List<OwnerViewModel>> GetOwners()
        {
            return await _ownerRepository.GetAll()
                .ProjectTo<OwnerViewModel>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<OwnerViewModel> GetOwner(Guid id)
        {
            return await _ownerRepository.GetMany(owner => owner.AccountId.Equals(id))
                .ProjectTo<OwnerViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy owner.");
        }

        public async Task<OwnerViewModel> CreateOwner(RegisterOwnerModel model)
        {
            var result = 0;
            var accountId = Guid.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    accountId = await _accountService.CreateAccount(model.PhoneNumber, model.Password, AccountRole.Owner);

                    var owner = new OwnerAccount
                    {
                        AccountId = accountId,
                        FullName = model.FullName,
                        Email = model.Email,
                    };

                    _ownerRepository.Add(owner);

                    result = await _unitOfWork.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };
            return result > 0 ? await GetOwner(accountId) : null!;
        }

        public async Task<OwnerViewModel> UpdateOwner(Guid id, UpdateOwnerModel model)
        {
            var owner = await _ownerRepository.GetMany(owner => owner.AccountId.Equals(id))
                                                .Include(owner => owner.Account)
                                                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy owner");

            owner.FullName = model.FullName ?? owner.FullName;
            owner.Account.Status = model.Status ?? owner.Account.Status;

            if (!string.IsNullOrEmpty(model.OldPassword))
            {
                if (!PasswordHasher.VerifyPassword(model.OldPassword, owner.Account.PasswordHash))
                {
                    throw new BadRequestException("Mật khẩu cũ không chính sát");
                }
                if (model.NewPassword != null)
                {
                    owner.Account.PasswordHash = PasswordHasher.HashPassword(model.NewPassword);
                }
            }
            _ownerRepository.Update(owner);


            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetOwner(owner.AccountId) : null!;
        }

        public async Task<OwnerViewModel> UploadAvatar(Guid id, IFormFile image)
        {

            if (!image.ContentType.StartsWith("image/"))
            {
                throw new BadRequestException("File không phải là hình ảnh");
            }

            var owner = await _ownerRepository.GetMany(owner => owner.AccountId.Equals(id)).FirstOrDefaultAsync();
            if (owner != null)
            {
                if (!string.IsNullOrEmpty(owner.Avatar))
                {
                    await _cloudStorageService.Delete(id);
                }

                var url = await _cloudStorageService.Upload(id, image.ContentType, image.OpenReadStream());

                owner.Avatar = url;

                _ownerRepository.Update(owner);
            }
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetOwner(id) : null!;
        }
    }
}
