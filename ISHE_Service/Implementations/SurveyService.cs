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
using Microsoft.EntityFrameworkCore;

namespace ISHE_Service.Implementations
{
    public class SurveyService : BaseService, ISurveyService
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IStaffAccountRepository _staffRepository;
        private readonly ISurveyRequestRepository _surveyRequestRepository;
        private readonly INotificationService _notificationService;
        private readonly ITellerAccountRepository _tellerAccountRepository;
        private readonly IDevicePackageRepository _devicePackageRepository;
        public SurveyService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService) : base(unitOfWork, mapper)
        {
            _surveyRepository = unitOfWork.Survey;
            _staffRepository = unitOfWork.StaffAccount;
            _surveyRequestRepository = unitOfWork.SurveyRequest;
            _notificationService = notificationService;
            _tellerAccountRepository = unitOfWork.TellerAccount;
            _devicePackageRepository = unitOfWork.DevicePackage;
        }

        public async Task<ListViewModel<SurveyViewModel>> GetSurveys(SurveyFilterModel filter, PaginationRequestModel pagination)
        {
            var query = _surveyRepository.GetAll();

            if (filter.StaffId.HasValue)
            {
                query = query.Where(sv => sv.SurveyRequest.StaffId == filter.StaffId.Value);
            }
            if (!string.IsNullOrEmpty(filter.CustomerName))
            {
                query = query.Where(sv => sv.SurveyRequest.Customer.FullName.Contains(filter.CustomerName));
            }

            if (filter.AppointmentDate.HasValue)
            {
                query = query.Where(sv => sv.AppointmentDate.Date.Equals(filter.AppointmentDate.Value.Date));
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

            var surveys = await paginatedQuery
                .ProjectTo<SurveyViewModel>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
            return new ListViewModel<SurveyViewModel>
            {
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalRow = totalRow
                },
                Data = surveys
            };
        }

        public async Task<SurveyViewModel> GetSurvey(Guid id)
        {
            return await _surveyRepository.GetMany(sv => sv.Id.Equals(id))
                .ProjectTo<SurveyViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy survey");
        }

        public async Task<SurveyViewModel> CreateSurvey(CreateSurveyModel model)
        {
            var surveyRequest = await _surveyRequestRepository.GetMany(sv => sv.Id.Equals(model.SurveyRequestId))
                .Include(sv => sv.Customer)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy survey request");
            if(surveyRequest.Status != SurveyRequestStatus.InProgress.ToString())
            {
                throw new BadRequestException($"Survey request status: {surveyRequest.Status}");
            }

            if (model.RecommendDevicePackageId.HasValue)
            {
                var devicePackage = await _devicePackageRepository.GetMany(d => d.Id == model.RecommendDevicePackageId).FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy device package");
                
                if (devicePackage.Status == DevicePackageStatus.InActive.ToString())
                {
                    throw new BadRequestException("Device package không còn hỗ trợ trên hệ thống");
                }
            }
            
            //await CheckStaffIsAvaiableForSurvey(model.StaffId, surveyRequest.SurveyDate);

            var survey = new Survey
            {
                Id = Guid.NewGuid(),
                SurveyRequestId = model.SurveyRequestId,
                RecommendDevicePackageId = model.RecommendDevicePackageId,
                RoomArea = model.RoomArea,
                Description = model.Description,
                AppointmentDate = model.AppointmentDate,
                Status = SurveyStatus.Pending.ToString(),
            };

            //sendNoti
            await SendNotificationToTeller(survey.Id, surveyRequest.Customer.FullName);

            _surveyRepository.Add(survey);

            surveyRequest.Status = SurveyRequestStatus.Completed.ToString();
            _surveyRequestRepository.Update(surveyRequest);

            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetSurvey(survey.Id) : null!;
        }

        public async Task<SurveyViewModel> UpdateSurvey(Guid id, UpdateSurveyModel model)
        {
            var survey = await _surveyRepository.GetMany(sv => sv.Id.Equals(id))
                    .Include(sv => sv.SurveyRequest)
                    .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy survey");

            survey.RecommendDevicePackageId = model.RecommendDevicePackageId ?? survey.RecommendDevicePackageId;
            survey.AppointmentDate = model.AppointmentDate ?? survey.AppointmentDate;
            survey.RoomArea = model.RoomArea ?? survey.RoomArea;
            survey.Description = model.Description ?? survey.Description;
            survey.Status = model.Status ?? survey.Status;

            _surveyRepository.Update(survey);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetSurvey(id) : null!;
        }

        //PRIVATE METHOD
        private async Task SendNotificationToTeller(Guid reportId, string customerName)
        {
            var message = new CreateNotificationModel
            {
                Title = $"Báo cáo hoàn thành khảo sát nhà",
                Body = $"Nhân viên đã hoàn thành thu thập thông tin nhà của khách hàng {customerName}. Vui lòng kiểm tra và xử lý báo cáo này.",
                Data = new NotificationDataViewModel
                {
                    CreateAt = DateTime.Now,
                    Type = NotificationType.SurveyReport,
                    Link = reportId.ToString()
                }
            };

            var tellers = await _tellerAccountRepository
                            .GetAll()
                            .Select(tl => tl.AccountId)
                            .ToListAsync();
            await _notificationService.SendNotification(tellers, message);
        }
    }
}
