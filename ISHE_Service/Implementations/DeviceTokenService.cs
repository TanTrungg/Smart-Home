using AutoMapper;
using ISHE_Data;
using ISHE_Data.Entities;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Repositories.Interfaces;
using ISHE_Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ISHE_Service.Implementations
{
    public class DeviceTokenService : BaseService, IDeviceTokenService
    {
        private readonly IDeviceTokenRepository _deviceToken;
        public DeviceTokenService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _deviceToken = unitOfWork.DeviceToken;
        }

        public async Task<bool> CreateDeviceToken(Guid accountId, CreateDeviceTokenModel model)
        {
            var deviceTokens = await _deviceToken.GetMany(token => token.AccountId.Equals(accountId)).ToListAsync();
            if (deviceTokens.Any(token => token.Token!.Equals(model.DeviceToken))) return false;

            var newDeviceToken = new DeviceToken
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                Token = model.DeviceToken
            };

            _deviceToken.Add(newDeviceToken);
            var result = await _unitOfWork.SaveChanges();
            return result > 0;
        }
    }
}
