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
using ISHE_Utility.Enum;
using ISHE_Utility.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ISHE_Service.Implementations
{
    public class PromotionService : BaseService, IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IDevicePackageRepository _devicePackageRepository;
        public PromotionService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _promotionRepository = unitOfWork.Promotion;
            _devicePackageRepository = unitOfWork.DevicePackage;
        }

        public async Task<ListViewModel<PromotionViewModel>> GetPromotions(PromotionFilterModel filter, PaginationRequestModel pagination)
        {
            var query = _promotionRepository.GetAll();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(promotion => promotion.Name.Contains(filter.Name));
            }

            if (!string.IsNullOrEmpty(filter.Status.ToString()))
            {
                query = query.Where(promotion => promotion.Status.Equals(filter.Status.ToString()));
            }

            var totalRow = await query.AsNoTracking().CountAsync();
            var paginatedQuery = query
                .OrderByDescending(promotion => promotion.CreateAt)
                .Skip(pagination.PageNumber * pagination.PageSize)
                .Take(pagination.PageSize);

            var promotions = await paginatedQuery
                .ProjectTo<PromotionViewModel>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
            return new ListViewModel<PromotionViewModel>
            {
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalRow = totalRow
                },
                Data = promotions
            };
        }

        public async Task<PromotionDetailViewModel> GetPromotion(Guid id)
        {
            return await _promotionRepository.GetMany(promotion => promotion.Id.Equals(id))
                .ProjectTo<PromotionDetailViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy promotion");
        }

        public async Task<PromotionDetailViewModel> CreatePromotion(CreatePromotionModel model)
        {
            var result = 0;
            var promotionId = Guid.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    CheckValidDate(model.StartDate, model.EndDate);

                    promotionId = Guid.NewGuid();
                    var promotion = new Promotion
                    {
                        Id = promotionId,
                        Name = model.Name,
                        DiscountAmount = model.DiscountAmount,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        Description = model.Description,
                        Status = PromotionStatus.Active.ToString()
                    };

                    _promotionRepository.Add(promotion);
                    await AddPromotionIdToDevicePackage(promotionId, model.DevicePackageIds, false);

                    result = await _unitOfWork.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };
            return result > 0 ? await GetPromotion(promotionId) : null!;
        }

        public async Task<PromotionDetailViewModel> UpdatePromotion(Guid id, UpdatePromotionModel model)
        {
            var promotion = await _promotionRepository.GetMany(pro => pro.Id.Equals(id))
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy promotion");

            promotion.Name = model.Name ?? promotion.Name;
            promotion.DiscountAmount = model.DiscountAmount ?? promotion.DiscountAmount;
            promotion.StartDate = model.StartDate ?? promotion.StartDate;
            promotion.EndDate = model.EndDate ?? promotion.EndDate;
            promotion.Description = model.Description ?? promotion.Description;
            promotion.Status = model.Status ?? promotion.Status;

            _promotionRepository.Update(promotion);
            await AddPromotionIdToDevicePackage(id, model.DevicePackageIds, true);

            var result = await _unitOfWork.SaveChanges();

            return result > 0 ? await GetPromotion(id) : null!;
        }

        public async Task CheckExpiredPromotion()
        {
            var currentTime = DateTime.Now;

            var expiredPromotion = await _promotionRepository
                    .GetMany(promotion => promotion.EndDate.Date < currentTime.Date && promotion.Status != PromotionStatus.Expired.ToString())
                    .ToListAsync();

            if (expiredPromotion.Count == 0) return;

            foreach (var discount in expiredPromotion)
            {
                discount.Status = PromotionStatus.Expired.ToString();
            }

            _promotionRepository.UpdateRange(expiredPromotion);
            await _unitOfWork.SaveChanges();
        }

        //PRIVATE METHOD
        private async Task<ICollection<DevicePackage>> AddPromotionIdToDevicePackage(Guid promotionId, List<Guid>? devicePackageIds, bool isUpdate)
        {
            var listDevicePackage = new List<DevicePackage>();
            if (devicePackageIds != null && devicePackageIds.Any())
            {
                if (isUpdate)
                {
                    var existDevicePackages = await _devicePackageRepository.GetMany(device => device.PromotionId.Equals(promotionId)).ToListAsync();
                    foreach (var devicePackage in existDevicePackages)
                    {
                        devicePackage.PromotionId = null;
                    }
                    _devicePackageRepository.UpdateRange(existDevicePackages);
                }

                foreach (var devicePackageId in devicePackageIds)
                {
                    var devicePackage = await _devicePackageRepository.GetMany(device => device.Id.Equals(devicePackageId))
                        .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy device package id: " + devicePackageId);

                    devicePackage.PromotionId = promotionId;
                    _devicePackageRepository.Update(devicePackage);

                    listDevicePackage.Add(devicePackage);
                }

            }
            return listDevicePackage;
        }

        private void CheckValidDate(DateTime startDate, DateTime endDate)
        {
            if (endDate < DateTime.Now)
            {
                throw new BadRequestException("Thời gian khuyến mãi phải lớn hơn hiện tại");
            }
            else if (startDate > endDate)
            {
                throw new BadRequestException("Thời gian khuyến mãi sai");
            }
        }
    }
}
