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

namespace ISHE_Service.Implementations
{
    public class CustomerService : BaseService, ICustomerService
    {
        private readonly ICustomerAccountRepository _customerRepository;

        private readonly IAccountService _accountService;
        private readonly ICloudStorageService _cloudStorageService;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, IAccountService accountService, ICloudStorageService cloudStorageService) : base(unitOfWork, mapper)
        {
            _customerRepository = unitOfWork.CustomerAccount;
            _accountService = accountService;
            _cloudStorageService = cloudStorageService;
        }

        public async Task<ListViewModel<CustomerViewModel>> GetCustomers(CustomerFilterModel filter, PaginationRequestModel pagination)
        {
            var query = _customerRepository.GetAll();

            if (!string.IsNullOrEmpty(filter.FullName))
            {
                query = query.Where(customer => customer.FullName.Contains(filter.FullName));
            }

            if (!string.IsNullOrEmpty(filter.PhoneNumber))
            {
                query = query.Where(customer => customer.Account.PhoneNumber.Contains(filter.PhoneNumber));
            }

            if (!string.IsNullOrEmpty(filter.Status.ToString()))
            {
                query = query.Where(customer => customer.Account.Status.Equals(filter.Status.ToString()));
            }

            var totalRow = await query.AsNoTracking().CountAsync();
            var paginatedQuery = query
                .OrderByDescending(customer => customer.Account.CreateAt)
                .Skip(pagination.PageNumber * pagination.PageSize)
                .Take(pagination.PageSize);
            var customers = await paginatedQuery
                .ProjectTo<CustomerViewModel>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
            return new ListViewModel<CustomerViewModel>
            {
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalRow = totalRow
                },
                Data = customers
            };
        }

        public async Task<CustomerViewModel> GetCustomer(Guid id)
        {
            return await _customerRepository.GetMany(customer => customer.AccountId.Equals(id))
                .ProjectTo<CustomerViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy customer.");
        }

        public async Task<CustomerViewModel> CreateCustomer(RegisterCustomerModel model)
        {
            var result = 0;
            var accountId = Guid.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    accountId = await _accountService.CreateAccount(model.PhoneNumber, model.Password, AccountRole.Customer);

                    var customer = new CustomerAccount
                    {
                        AccountId = accountId,
                        FullName = model.FullName,
                        Email = model.Email,
                        Address = model.Address,
                    };

                    _customerRepository.Add(customer);

                    result = await _unitOfWork.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };
            return result > 0 ? await GetCustomer(accountId) : null!;
        }

        public async Task<CustomerViewModel> UpdateCustomer(Guid id, UpdateCustomerModel model)
        {
            var customer = await _customerRepository.GetMany(staff => staff.AccountId.Equals(id))
                                                .Include(staff => staff.Account)
                                                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy customer");

            customer.FullName = model.FullName ?? customer.FullName;
            customer.Account.Status = model.Status ?? customer.Account.Status;
            customer.Address = model.Address ?? customer.Address;

            if (!string.IsNullOrEmpty(model.OldPassword))
            {
                if (!PasswordHasher.VerifyPassword(model.OldPassword, customer.Account.PasswordHash))
                {
                    throw new BadRequestException("Mật khẩu cũ không chính sát.");
                }
                if (model.NewPassword != null)
                {
                    customer.Account.PasswordHash = PasswordHasher.HashPassword(model.NewPassword);
                }
            }
            _customerRepository.Update(customer);

            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetCustomer(customer.AccountId) : null!;
        }

        public async Task<CustomerViewModel> UploadAvatar(Guid id, IFormFile image)
        {
            if (!image.ContentType.StartsWith("image/"))
            {
                throw new BadRequestException("File không phải là hình ảnh");
            }
            var customer = await _customerRepository.GetMany(customer => customer.AccountId.Equals(id)).FirstOrDefaultAsync();
            if (customer != null)
            {
                //xóa hình cũ trong firebase
                if (!string.IsNullOrEmpty(customer.Avatar))
                {
                    await _cloudStorageService.Delete(id);
                }

                //upload hình mới
                var url = await _cloudStorageService.Upload(id, image.ContentType, image.OpenReadStream());

                customer.Avatar = url;

                _customerRepository.Update(customer);
            }
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetCustomer(id) : null!;
        }
    }
}
