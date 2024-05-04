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
using System.Diagnostics.Contracts;

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
                    
                    var packages = await AddDevicePackage(model.DevicePackageIds, model.StartDate, model.EndDate) ?? new List<DevicePackage>();
                    promotionId = Guid.NewGuid();
                    var promotion = new Promotion
                    {
                        Id = promotionId,
                        Name = model.Name,
                        DiscountAmount = model.DiscountAmount,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        Description = model.Description,
                        DevicePackages = packages,
                        Status = PromotionStatus.InActive.ToString()
                    };

                    _promotionRepository.Add(promotion);
                    //await AddPromotionIdToDevicePackage(promotionId, model.DevicePackageIds, false);

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
                .Include(x => x.DevicePackages)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy promotion");

            

            promotion.Name = model.Name ?? promotion.Name;
            promotion.DiscountAmount = model.DiscountAmount ?? promotion.DiscountAmount;
            //promotion.StartDate = model.StartDate ?? promotion.StartDate;
            //promotion.EndDate = model.EndDate ?? promotion.EndDate;
            //CheckValidDate(promotion.StartDate, promotion.EndDate);
            if (model.EndDate.HasValue)
            {
                if(promotion.Status == PromotionStatus.Expired.ToString())
                {
                    throw new BadRequestException("Khuyến mãi đã hết hạn");
                }
                if (model.EndDate <= DateTime.Now)
                {
                    throw new BadRequestException("Ngày kết thúc phải lớn hơn thời gian hiện tại");
                }
                if(model.EndDate < promotion.StartDate)
                {
                    throw new BadRequestException("Ngày kết thúc phải lớn hơn ngày bắt đầu");
                }

                promotion.EndDate = model.EndDate.Value;
            }
            

            promotion.Description = model.Description ?? promotion.Description;
            if (!string.IsNullOrEmpty(model.Status))
            {
                UpdateStatus(promotion, model.Status);
            }
            promotion.Status = model.Status ?? promotion.Status;
            
            if (model.DevicePackageIds != null && model.DevicePackageIds.Any())
            {
                var packages = await UpdateDevicePackage(model.DevicePackageIds, promotion.StartDate, promotion.EndDate, id) ?? new List<DevicePackage>();
                promotion.DevicePackages.Clear();
                promotion.DevicePackages = packages;
            }
            _promotionRepository.Update(promotion);
            //await AddPromotionIdToDevicePackage(id, model.DevicePackageIds, true);

            var result = await _unitOfWork.SaveChanges();

            return result > 0 ? await GetPromotion(id) : null!;
        }

        private void UpdateStatus(Promotion promotion, string newStatus)
        {
            switch (promotion.Status)
            {
                case nameof(PromotionStatus.Active):
                    if(newStatus == nameof(PromotionStatus.Expired))
                    {
                        promotion.Status = newStatus;
                        promotion.EndDate = DateTime.Now;
                    }
                    else
                    {
                        throw new BadRequestException($"Không thể cập nhật trạng thái từ {promotion.Status} thành {newStatus}");
                    }
                        break;
                //case nameof(PromotionStatus.InActive):
                //case nameof(PromotionStatus.Expired):
                //    promotion.Status = newStatus;
                //    break;
                default:
                    throw new BadRequestException($"Không thể cập nhật trạng thái từ {promotion.Status} thành {newStatus}");
            }
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

        public async Task CheckActivePromotion()
        {
            var currentTime = DateTime.Now;

            var activePromotion = await _promotionRepository
                    .GetMany(promotion => promotion.StartDate.Date <= currentTime.Date && promotion.Status == PromotionStatus.InActive.ToString())
                    .ToListAsync();

            if (activePromotion.Count == 0) return;

            foreach (var discount in activePromotion)
            {
                discount.Status = PromotionStatus.Active.ToString();
            }

            _promotionRepository.UpdateRange(activePromotion);
            await _unitOfWork.SaveChanges();
        }

        //PRIVATE METHOD

        private async Task<List<DevicePackage>?> AddDevicePackage(List<Guid>? devicePackageIds, DateTime startDate, DateTime endDate)
        {
            CheckValidDate(startDate, endDate);

            var result = new List<DevicePackage>();
            if (devicePackageIds != null && devicePackageIds.Any())
            {
                var uniqueIds = new HashSet<Guid>();
                foreach (var devicePackageId in devicePackageIds)
                {
                    if (uniqueIds.Add(devicePackageId))
                    {
                        var package = await _devicePackageRepository.GetMany(d => d.Id == devicePackageId)
                            .Include(p => p.Promotions)
                            .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy package");
                        //check điều kiện invalue
                        if(package.Status == DevicePackageStatus.InActive.ToString())
                        {
                            throw new BadRequestException($"Gói {package.Name} đã không còn được hỗ trợ");
                        }

                        if(package.Promotions != null && package.Promotions.Count > 0)
                        {
                            var overLapping = package.Promotions.FirstOrDefault(pro => pro.StartDate <= endDate && pro.EndDate >= startDate);
                            if(overLapping != null)
                            {
                                throw new ConflictException($"Gói {package.Name} đã có chương trình khuyến mãi trong thời gian trên");
                            }
                        }

                        result.Add(package);
                    }
                }
            }
            return result;
        }

        private async Task<List<DevicePackage>?> UpdateDevicePackage(List<Guid>? devicePackageIds, DateTime startDate, DateTime endDate, Guid promotionId)
        {
            //CheckValidDate(startDate, endDate);

            var result = new List<DevicePackage>();
            if (devicePackageIds != null && devicePackageIds.Any())
            {
                var uniqueIds = new HashSet<Guid>();
                foreach (var devicePackageId in devicePackageIds)
                {
                    if (uniqueIds.Add(devicePackageId))
                    {
                        var package = await _devicePackageRepository.GetMany(d => d.Id == devicePackageId)
                            .Include(p => p.Promotions)
                            .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy package");
                        //check điều kiện invalue
                        if (package.Status == DevicePackageStatus.InActive.ToString())
                        {
                            throw new BadRequestException($"Gói {package.Name} đã không còn được hỗ trợ");
                        }

                        if (package.Promotions != null && package.Promotions.Count > 0)
                        {
                            var overLapping = package.Promotions.FirstOrDefault(pro => pro.Id != promotionId && (pro.StartDate <= endDate && pro.EndDate >= startDate));
                            if (overLapping != null)
                            {
                                throw new ConflictException($"Gói {package.Name} đã có chương trình khuyến mãi trong thời gian trên");
                            }
                        }

                        result.Add(package);
                    }
                }
            }
            return result;
        }


        //private async Task<ICollection<DevicePackage>> AddPromotionIdToDevicePackage(Guid promotionId, List<Guid>? devicePackageIds, bool isUpdate)
        //{
        //    var listDevicePackage = new List<DevicePackage>();
        //    if (devicePackageIds != null && devicePackageIds.Any())
        //    {
        //        if (isUpdate)
        //        {
        //            var existDevicePackages = await _devicePackageRepository.GetMany(device => device.PromotionId.Equals(promotionId)).ToListAsync();
        //            foreach (var devicePackage in existDevicePackages)
        //            {
        //                devicePackage.PromotionId = null;
        //            }
        //            _devicePackageRepository.UpdateRange(existDevicePackages);
        //        }

        //        foreach (var devicePackageId in devicePackageIds)
        //        {
        //            var devicePackage = await _devicePackageRepository.GetMany(device => device.Id.Equals(devicePackageId))
        //                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy device package id: " + devicePackageId);

        //            devicePackage.PromotionId = promotionId;
        //            _devicePackageRepository.Update(devicePackage);

        //            listDevicePackage.Add(devicePackage);
        //        }

        //    }
        //    return listDevicePackage;
        //}

        private void CheckValidDate(DateTime startDate, DateTime endDate)
        {
            if(startDate <= DateTime.Now.Date)
            {
                throw new BadRequestException("Thời gian bắt đầu phải lớn hơn hiện tại");
            }

            if (endDate <= startDate)
            {
                throw new BadRequestException("Thời gian kết thúc khuyến mãi phải lớn ngày bắt đầu");
            }
            //else if (startDate > endDate)
            //{
            //    throw new BadRequestException("Thời gian khuyến mãi sai");
            //}
        }
    }
}
