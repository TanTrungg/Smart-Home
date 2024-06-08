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
using Microsoft.EntityFrameworkCore;

namespace ISHE_Service.Implementations
{
    public class SurveyRequestService : BaseService, ISurveyRequestService
    {
        private readonly ISurveyRequestRepository _surveyRequestRepository;
        private readonly IStaffAccountRepository _staffAccountRepository;
        private readonly ITellerAccountRepository _tellerAccountRepository;
        private readonly ICustomerAccountRepository _customerAccount;

        private readonly ISendMailService _sendMailService;

        private readonly INotificationService _notificationService;
        public SurveyRequestService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService, ISendMailService sendMailService) : base(unitOfWork, mapper)
        {
            _surveyRequestRepository = unitOfWork.SurveyRequest;
            _staffAccountRepository = unitOfWork.StaffAccount;
            _tellerAccountRepository = unitOfWork.TellerAccount;
            _customerAccount = unitOfWork.CustomerAccount;
            _notificationService = notificationService;
            _sendMailService = sendMailService;
        }

        public async Task<ListViewModel<SurveyRequestViewModel>> GetSurveyRequests(SurveyRequestFilterModel filter, PaginationRequestModel pagination)
        {
            var query = _surveyRequestRepository.GetAll();

            if (filter.CustomerId.HasValue)
            {
                query = query.Where(sv => sv.CustomerId.Equals(filter.CustomerId.Value));
            }

            if (filter.StaffId.HasValue)
            {
                query = query.Where(sv => sv.StaffId.Equals(filter.StaffId.Value));
            }

            if (filter.SurveyDate.HasValue)
            {
                query = query.Where(sv => sv.SurveyDate.Date.Equals(filter.SurveyDate.Value.Date));
            }

            if (!string.IsNullOrEmpty(filter.Status.ToString()))
            {
                query = query.Where(sv => sv.Status.Equals(filter.Status.ToString()));
            }

            var totalRow = await query.AsNoTracking().CountAsync();
            var paginatedQuery = query
                .OrderByDescending(sv => sv.CreateAt)
                .Skip(pagination.PageNumber * pagination.PageSize)
                .Take(pagination.PageSize);

            var surveyRequests = await paginatedQuery
                .ProjectTo<SurveyRequestViewModel>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
            return new ListViewModel<SurveyRequestViewModel>
            {
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalRow = totalRow
                },
                Data = surveyRequests
            };
        }

        public async Task<SurveyRequestViewModel> GetSurveyRequest(Guid id)
        {
            return await _surveyRequestRepository.GetMany(sv => sv.Id.Equals(id))
                .ProjectTo<SurveyRequestViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy survey request");
        }

        public async Task<SurveyRequestViewModel> CreateSurveyRequest(CreateSurveyRequestModel model)
        {
            var surveyDate = FormatDate.CheckFormatDate(model.SurveyDate);
            IsValidDateToSurvey(surveyDate);
            await CheckCustomerRequest(model.CustomerId, surveyDate);

            var request = new SurveyRequest
            {
                Id = Guid.NewGuid(),
                CustomerId = model.CustomerId,
                SurveyDate = surveyDate,
                Description = model.Description,
                Status = SurveyRequestStatus.Pending.ToString()
            };

            //Send noti
            await SendNotificationToTeller(request);
            _surveyRequestRepository.Add(request);

           

            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetSurveyRequest(request.Id) : null!;
        }

        public async Task<SurveyRequestViewModel> UpdateSurveyRequest(Guid id, UpdateSurveyRequestModel model)
        {
            var request = await _surveyRequestRepository.GetMany(sv => sv.Id.Equals(id))
                                        .Include(x => x.Customer)
                                        .Include(x => x.Staff)
                                        .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thầy survey request");

            if (!string.IsNullOrEmpty(model.SurveyDate))
            {
                var surveyDate = FormatDate.CheckFormatDate(model.SurveyDate);
                if (!request.SurveyDate.Date.Equals(surveyDate.Date))
                {
                    IsValidDateToSurvey(surveyDate);
                    await CheckCustomerRequest(request.CustomerId, surveyDate);
                    request.SurveyDate = surveyDate;
                }
            }

            if (!string.IsNullOrEmpty(model.Status))
            {
                //if (IsValidStatus(request.Status, model.Status))
                //{
                    request.Status = model.Status;
                //}
                //else
                //{
                //    throw new BadRequestException($"Không thể cập nhật trạng thái từ {request.Status} thành {model.Status}");
                //}
            }
            
            request.Description = model.Description ?? request.Description;

            if (model.StaffId.HasValue && model.StaffId.Value != request.StaffId)
            {
                await CheckStaffIsAvaiableForSurvey(model.StaffId.Value, request.SurveyDate.Date);
                request.StaffId = model.StaffId.Value;
                request.Status = SurveyRequestStatus.InProgress.ToString();

                await SendNotificationToStaff(request);
                await SendNotificationToCustomer(request);
            }

            _surveyRequestRepository.Update(request);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetSurveyRequest(id) : null!;
        }

        //PRIVATE METHOD
        private bool IsValidStatus(string currentStatus, string newStatus)
        {
            switch (currentStatus)
            {
                case nameof(SurveyRequestStatus.Pending):
                    return newStatus == nameof(SurveyRequestStatus.Rejected);
                case nameof(SurveyRequestStatus.InProgress):
                case nameof(SurveyRequestStatus.Completed):
                    return false;
                default:
                    return false;
            }
        }

        private async Task CheckCustomerRequest(Guid customerId, DateTime requestDate)
        {
            var existingRequest = await _surveyRequestRepository.GetMany(request => request.CustomerId.Equals(customerId)
                                                                && request.SurveyDate.Date.Equals(requestDate.Date))
                                                        .Include(request => request.Customer)
                                                        .FirstOrDefaultAsync();
            if (existingRequest != null)
            {
                throw new BadRequestException($"Customer {existingRequest.Customer.FullName} đã có lịch hẹn survey cho ngày {requestDate.ToString("dd-MM-yyyy")}");
            }
        }

        private async Task CheckStaffIsAvaiableForSurvey(Guid staffId, DateTime surveyDate)
        {
            
            var staffAccount = await _staffAccountRepository.GetMany(staff => staff.AccountId.Equals(staffId))
                .Include(sv => sv.Contracts)
                .Include(sv => sv.SurveyRequests)
                .FirstOrDefaultAsync() ?? throw new BadRequestException($"Không tìm thấy nhân viên với ID {staffId}");

            if (!staffAccount.IsLead ||
                staffAccount.SurveyRequests.Count(sv => sv.SurveyDate.Date == surveyDate.Date) >= 3)
            {
                throw new BadRequestException($"Nhân viên {staffAccount.FullName} không phù hợp để tham gia khảo sát vào ngày {surveyDate.ToString("dd-MM-yyyy")}");
            }
        }

        private void IsValidDateToSurvey(DateTime requestDate)
        {
            var currentDate = DateTime.Now.Date;
            if(requestDate.Date <= currentDate)
            {
                throw new BadRequestException("Ngày khảo sát phải là ngày trong tương lai");
            }
        }

        private async Task SendNotificationToTeller(SurveyRequest request)
        {
            var message = new CreateNotificationModel
            {
                Title = $"Yêu cầu khảo sát nhà từ khách hàng",
                Body = $"Có một yêu cầu khảo sát mới từ khách hành. Vui lòng tiếp nhận.",
                Data = new NotificationDataViewModel
                {
                    CreateAt = DateTime.Now,
                    Type = NotificationType.SurveyRequest,
                    Link = request.Id.ToString()
                }
            };
            var tellers = await _tellerAccountRepository
                            .GetAll()
                            .Select(tl => tl.AccountId)
                            .ToListAsync();
            await _notificationService.SendNotification(tellers , message);
        }

        private async Task SendNotificationToStaff(SurveyRequest request)
        {
            var message = new CreateNotificationModel
            {
                Title = $"Yêu cầu khảo sát nhà từ teller",
                Body = $"Có một yêu cầu khảo sát nhà từ khách hàng {request.Customer.FullName} đã được bàn giao cho bạn. Vui lòng tiếp nhận và tiến hành khảo sát.",
                Data = new NotificationDataViewModel
                {
                    CreateAt = DateTime.Now,
                    Type = NotificationType.SurveyRequest,
                    Link = request.Id.ToString()
                }
            };
            
            await _notificationService.SendNotification(new List<Guid> { (Guid)request.StaffId! }, message);

            await _sendMailService.SendEmail(request.Staff!.Email!, message.Title, message.Body);
        }

        private async Task SendNotificationToCustomer(SurveyRequest request)
        {
            var message = new CreateNotificationModel
            {
                Title = $"Yêu cầu khảo sát nhà đã được tiếp nhận",
                Body = $"Yêu cầu khảo sát nhà của bạn đã được tiếp nhận và bàn giao cho nhân viên {request.Staff!.FullName}. " +
                $"Nhân viên sẽ liên hệ cho bạn vào ngày {request.SurveyDate} để tiến hành khảo sát. Chân thành cảm ơn bạn đã tin tưởng và sử dụng dịch vụ bên chúng tôi.",
                Data = new NotificationDataViewModel
                {
                    CreateAt = DateTime.Now,
                    Type = NotificationType.SurveyRequest,
                    Link = request.Id.ToString()
                }
            };

            await _notificationService.SendNotification(new List<Guid> { (Guid)request.CustomerId! }, message);

            var email = await _customerAccount.GetMany(s => s.AccountId == request.CustomerId).Select(e => e.Email).FirstOrDefaultAsync();
            if (email != null)
            {
                await _sendMailService.SendEmail(email, message.Title, message.Body);
            }
        }
    }
}
