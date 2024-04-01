using AutoMapper;
using AutoMapper.QueryableExtensions;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using ISHE_Data;
using ISHE_Data.Models.Requests.Get;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;
using ISHE_Data.Repositories.Interfaces;
using ISHE_Service.Interfaces;
using ISHE_Utility.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ISHE_Service.Implementations
{
    public class NotificationService : BaseService, INotificationService
    {
        private readonly IDeviceTokenRepository _deviceTokenRepository;
        private readonly INotificationRepository _notificationRepository;
        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _deviceTokenRepository = unitOfWork.DeviceToken;
            _notificationRepository = unitOfWork.Notification;
        }

        public async Task<NotificationViewModel> GetNotification(Guid id)
        {
            return await _notificationRepository.GetMany(notification => notification.Id.Equals(id))
                .ProjectTo<NotificationViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy thông báo.");
        }

        public async Task<ListViewModel<NotificationViewModel>> GetNotifications(Guid accountId, PaginationRequestModel pagination)
        {
            var query = _notificationRepository.GetMany(noti => noti.AccountId.Equals(accountId));
            
            var totalRow = await query.AsNoTracking().CountAsync();
            var paginatedQuery = query
                .OrderByDescending(noti => noti.CreateAt)
                .Skip(pagination.PageNumber * pagination.PageSize)
                .Take(pagination.PageSize);
            var nonotifications = await paginatedQuery
                .ProjectTo<NotificationViewModel>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            return new ListViewModel<NotificationViewModel>
            {
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalRow = totalRow
                },
                Data = nonotifications
            };
        }

        public async Task<List<string?>> GetDeviceToken(Guid accountId)
        {
            return await _deviceTokenRepository.GetMany(token => token.AccountId.Equals(accountId))
                .Select(token => token.Token)
                .ToListAsync();
        }

        public async Task<bool> SendNotification(ICollection<Guid> accountIds, CreateNotificationModel model)
        {
            var deviceTokens = await _deviceTokenRepository.GetMany(token => accountIds.Contains(token.AccountId))
                .Select(token => token.Token)
                .ToListAsync();
            var now = DateTime.UtcNow.AddHours(7);
            foreach (var accountId in accountIds)
            {
                var notification = new ISHE_Data.Entities.Notification
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    CreateAt = now,
                    Body = model.Body,
                    Type = model.Data.Type,
                    Link = model.Data.Link,
                    Title = model.Title
                };
                _notificationRepository.Add(notification);
            }

            var result = await _unitOfWork.SaveChanges();
            if (result > 0)
            {
                if (deviceTokens.Any())
                {
                    var messageData = new Dictionary<string, string>
                    {
                        { "type", model.Data.Type ?? "" },
                        { "link", model.Data.Link ?? "" },
                        { "createAt", now.ToString() }
                    };
                    var message = new MulticastMessage()
                    {
                        Notification = new FirebaseAdmin.Messaging.Notification()
                        {
                            Title = model.Title,
                            Body = model.Body
                        },
                        Data = messageData,
                        Tokens = deviceTokens
                    };
                    var app = FirebaseApp.DefaultInstance;
                    if (FirebaseApp.DefaultInstance == null)
                    {
                        GoogleCredential credential;
                        var credentialJson = Environment.GetEnvironmentVariable("GoogleCloudCredential");
                        if (string.IsNullOrWhiteSpace(credentialJson))
                        {
                            var basePath = AppDomain.CurrentDomain.BaseDirectory;
                            var projectRoot = Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", ".."));
                            string credentialPath = Path.Combine(projectRoot, "ISHE_Utility", "Helpers", "CloudStorage", "smarthome-856d3-firebase-adminsdk-88tdt-cc841847e7.json");
                            credential = GoogleCredential.FromFile(credentialPath);
                        }
                        else
                        {
                            credential = GoogleCredential.FromJson(credentialJson);
                        }

                        app = FirebaseApp.Create(new AppOptions()
                        {
                            Credential = credential
                        });
                    }
                    FirebaseMessaging messaging = FirebaseMessaging.GetMessaging(app);
                    await messaging.SendMulticastAsync(message);
                }
            }
            return true;
        }

        public async Task<NotificationViewModel> UpdateNotification(Guid id, UpdateNotificationModel model)
        {
            var notification = await _notificationRepository.GetMany(notification => notification.Id.Equals(id)).FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy thông báo");

            notification.IsRead = true;
            _notificationRepository.Update(notification);

            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetNotification(id) : null!;
        }

        public async Task<bool> MakeAsRead(Guid accountId)
        {
            var notifications = await _notificationRepository.GetMany(notification => notification.AccountId.Equals(accountId)).ToListAsync();
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }
            _notificationRepository.UpdateRange(notifications);
            var result = await _unitOfWork.SaveChanges();
            return result > 0;
        }

        public async Task<bool> DeleteNotification(Guid id)
        {
            var notification = await _notificationRepository.GetMany(noti => noti.Id.Equals(id)).FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy thông báo");

            _notificationRepository.Remove(notification);
            var result = await _unitOfWork.SaveChanges();
            return result > 0;
        }
    }
}
