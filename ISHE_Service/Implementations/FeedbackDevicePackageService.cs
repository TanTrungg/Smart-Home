using AutoMapper;
using AutoMapper.QueryableExtensions;
using ISHE_Data;
using ISHE_Data.Entities;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;
using ISHE_Data.Repositories.Interfaces;
using ISHE_Service.Interfaces;
using ISHE_Utility.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISHE_Service.Implementations
{
    public class FeedbackDevicePackageService : BaseService, IFeedbackDevicePackageService
    {
        private readonly IFeedbackDevicePackageRepository _feedback;
        private readonly ICustomerAccountRepository _customer;
        private readonly IDevicePackageRepository _device;
        
        public FeedbackDevicePackageService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _feedback = unitOfWork.FeedbackDevicePackage;
            _customer = unitOfWork.CustomerAccount;
            _device = unitOfWork.DevicePackage;
        }

        public async Task<FeedbackDevicePackageViewModel> GetFeedback(Guid id)
        {
            return await _feedback.GetMany(fb => fb.Id.Equals(id))
                .ProjectTo<FeedbackDevicePackageViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy feedback");
        }

        public async Task<FeedbackDevicePackageViewModel> CreateFeedback(CreateFeedbackDevicePackageModel model)
        {
            await CheckId(model.CustomerId, model.DevicePackageId);
            var flag = await _feedback.GetMany(fb => fb.DevicePackageId.Equals(model.DevicePackageId)
                                            && fb.CustomerId.Equals(model.CustomerId))
                                .FirstOrDefaultAsync();
            if(flag != null)
            {
                throw new BadRequestException("Khách hàng đã feedback cho sản phẩm này rồi");
            }

            var feedbackId = Guid.NewGuid();
            var feedback = new FeedbackDevicePackage
            {
                Id = feedbackId,
                CustomerId = model.CustomerId,
                DevicePackageId = model.DevicePackageId,
                Rating = model.Rating,
                Content = model.Content
            };
            _feedback.Add(feedback);

            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetFeedback(feedbackId) : null!;
        }

        private async Task CheckId(Guid customerId, Guid devicePackageId)
        {
            var flag1 = await _customer.GetMany(cus => cus.AccountId == customerId).FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy customer");
            var flag2 = await _device.GetMany(cus => cus.Id == devicePackageId).FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy device package");
        }

        public async Task<FeedbackDevicePackageViewModel> UpdateFeedBack(Guid id, UpdateFeedbackDevicePackageModel model)
        {
            var feedback = await _feedback.GetMany(fb => fb.Id.Equals(id))
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy feedback");

            feedback.Rating = model.Rating ?? feedback.Rating;
            feedback.Content = model.Content ?? feedback.Content;

            _feedback.Update(feedback);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetFeedback(id) : null!;
        }
    }
}
