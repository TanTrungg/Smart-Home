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
using ISHE_Utility.Enum;
using ISHE_Utility.Exceptions;
using ISHE_Utility.Helpers.FormatDate;
using ISHE_Utility.Helpers.PasswordHasher;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ISHE_Service.Implementations
{
    public class StaffService : BaseService, IStaffService
    {
        private readonly IStaffAccountRepository _staffRepository;

        private readonly IAccountService _accountService;
        private readonly ICloudStorageService _cloudStorageService;
        public StaffService(IUnitOfWork unitOfWork, IMapper mapper, IAccountService accountService, ICloudStorageService cloudStorageService) : base(unitOfWork, mapper)
        {
            _staffRepository = unitOfWork.StaffAccount;
            _accountService = accountService;
            _cloudStorageService = cloudStorageService;
        }

        public async Task<ListViewModel<StaffViewModel>> GetStaffs(StaffFilterModel filter, PaginationRequestModel pagination)
        {
            var query = _staffRepository.GetAll();

            if (!string.IsNullOrEmpty(filter.FullName))
            {
                query = query.Where(staff => staff.FullName.Contains(filter.FullName));
            }

            if (!string.IsNullOrEmpty(filter.PhoneNumber))
            {
                query = query.Where(staff => staff.Account.PhoneNumber.Contains(filter.PhoneNumber));
            }

            if (!string.IsNullOrEmpty(filter.Status.ToString()))
            {
                query = query.Where(staff => staff.Account.Status.Equals(filter.Status.ToString()));
            }

            var totalRow = await query.AsNoTracking().CountAsync();
            var paginatedQuery = query
                .OrderByDescending(staff => staff.Account.CreateAt)
                .Skip(pagination.PageNumber * pagination.PageSize)
                .Take(pagination.PageSize);
            var staffs = await paginatedQuery
                .ProjectTo<StaffViewModel>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
            return new ListViewModel<StaffViewModel>
            {
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalRow = totalRow
                },
                Data = staffs
            };
        }

        public async Task<ListViewModel<StaffGroupViewModel>> GetStaffLeads(StaffFilterModel filter, PaginationRequestModel pagination)
        {
            var query = _staffRepository.GetMany(staff => staff.IsLead.Equals(true));

            if (!string.IsNullOrEmpty(filter.FullName))
            {
                query = query.Where(staff => staff.FullName.Contains(filter.FullName));
            }

            if (!string.IsNullOrEmpty(filter.PhoneNumber))
            {
                query = query.Where(staff => staff.Account.PhoneNumber.Contains(filter.PhoneNumber));
            }

            if (!string.IsNullOrEmpty(filter.Status.ToString()))
            {
                query = query.Where(staff => staff.Account.Status.Equals(filter.Status.ToString()));
            }

            var totalRow = await query.AsNoTracking().CountAsync();
            var paginatedQuery = query
                .OrderByDescending(staff => staff.Account.CreateAt)
                .Skip(pagination.PageNumber * pagination.PageSize)
                .Take(pagination.PageSize);
            var staffs = await paginatedQuery
                .ProjectTo<StaffGroupViewModel>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
            return new ListViewModel<StaffGroupViewModel>
            {
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalRow = totalRow
                },
                Data = staffs
            };
        }

        public async Task<List<StaffViewModel>> GetStaffsAvaiableForSurey(StaffLeadRequestModel request)
        {
            var requestDate = FormatDate.CheckFormatDate(request.SurveyDate);
            var eligibleStaffLeads = await _staffRepository.GetMany(staff =>
                staff.IsLead &&
                staff.SurveyRequests.Count(sr => sr.SurveyDate.Date == requestDate.Date) < 3
            )
            .ProjectTo<StaffViewModel>(_mapper.ConfigurationProvider)
            .ToListAsync();

            return eligibleStaffLeads;
        }

        public async Task<StaffViewModel> GetStaff(Guid id)
        {
            return await _staffRepository.GetMany(staff => staff.AccountId.Equals(id))
                .ProjectTo<StaffViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy staff.");
        }

        public async Task<StaffViewModel> CreateStaff(RegisterStaffModel model)
        {
            var result = 0;
            var accountId = Guid.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    accountId = await _accountService.CreateAccount(model.PhoneNumber, model.Password, AccountRole.Staff);

                    var staff = new StaffAccount
                    {
                        AccountId = accountId,
                        FullName = model.FullName,
                        Email = model.Email,
                        IsLead = true
                    };

                    if (!model.IsLead && model.StaffLeadId.HasValue)
                    {
                        staff.IsLead = false;
                        var flag = await IsStaffLead(model.StaffLeadId.Value);
                        if (!flag)
                        {
                            throw new BadRequestException("Không tìm thấy Staff Leader đã chọn");
                        }
                        staff.StaffLeadId = model.StaffLeadId.Value;
                    }

                    _staffRepository.Add(staff);

                    result = await _unitOfWork.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };
            return result > 0 ? await GetStaff(accountId) : null!;
        }

        public async Task<StaffViewModel> UpdateStaff(Guid id, UpdateStaffModel model)
        {
            var staff = await _staffRepository.GetMany(staff => staff.AccountId.Equals(id))
                                                .Include(staff => staff.Account)
                                                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy staff");

            staff.FullName = model.FullName ?? staff.FullName;
            staff.Account.Status = model.Status ?? staff.Account.Status;

            if (!string.IsNullOrEmpty(model.OldPassword))
            {
                if (!PasswordHasher.VerifyPassword(model.OldPassword, staff.Account.PasswordHash))
                {
                    throw new BadRequestException("Mật khẩu cũ không chính sát.");
                }
                if (model.NewPassword != null)
                {
                    staff.Account.PasswordHash = PasswordHasher.HashPassword(model.NewPassword);
                }
            }
            _staffRepository.Update(staff);

            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetStaff(staff.AccountId) : null!;
        }

        public async Task<StaffViewModel> UploadAvatar(Guid id, IFormFile image)
        {
            if (!image.ContentType.StartsWith("image/"))
            {
                throw new BadRequestException("File không phải là hình ảnh");
            }
            var staff = await _staffRepository.GetMany(staff => staff.AccountId.Equals(id)).FirstOrDefaultAsync();
            if (staff != null)
            {
                //xóa hình cũ trong firebase
                if (!string.IsNullOrEmpty(staff.Avatar))
                {
                    await _cloudStorageService.Delete(id);
                }

                //upload hình mới
                var url = await _cloudStorageService.Upload(id, image.ContentType, image.OpenReadStream());

                staff.Avatar = url;

                _staffRepository.Update(staff);
            }
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetStaff(id) : null!;
        }

        //PRIVATE METHOD
        private async Task<bool> IsStaffLead(Guid? id)
        {
            return await _staffRepository.GetMany(staff => staff.AccountId.Equals(id) && staff.IsLead).AnyAsync(); ;
        }
    }
}
