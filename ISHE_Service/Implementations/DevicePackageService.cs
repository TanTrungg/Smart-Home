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
using ISHE_Utility.Helpers.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ISHE_Service.Implementations
{
    public class DevicePackageService : BaseService, IDevicePackageService
    {
        private readonly IDevicePackageRepository _packageRepository;
        private readonly ISmartDeviceRepository _deviceRepository;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IImageRepository _imageRepository;
        private readonly ISmartDevicePackageRepository _smartDevicePackage;
        private readonly IDevicePackageUsageRepository _devicePackageUsageRepository;

        private readonly ICloudStorageService _cloudStorageService;

        public DevicePackageService(IUnitOfWork unitOfWork, IMapper mapper, ICloudStorageService cloudStorageService) : base(unitOfWork, mapper)
        {
            _packageRepository = unitOfWork.DevicePackage;
            _deviceRepository = unitOfWork.SmartDevice;
            _manufacturerRepository = unitOfWork.Manufacturer;
            _promotionRepository = unitOfWork.Promotion;
            _imageRepository = unitOfWork.Image;
            _smartDevicePackage = unitOfWork.SmartDevicePackage;
            _devicePackageUsageRepository = unitOfWork.DevicePackageUsage;

            _cloudStorageService = cloudStorageService;
        }

        public async Task<List<MostDevicePackageViewModel>> GetMostDevicePackages()
        {
            var mostDevicePackage = await _devicePackageUsageRepository.GetAll()
                .GroupBy(package => package.DevicePackageId)
                .Select(package => new
                {
                    DevicePakageId = package.Key,
                    TotalSold = package.Count()
                })
                .OrderByDescending(package => package.TotalSold)
                .Take(10).
                ToListAsync();

            var result = new List<MostDevicePackageViewModel>();
            foreach(var package in mostDevicePackage)
            {
                var pac = await _packageRepository.GetMany(p => p.Id == package.DevicePakageId)
                    .ProjectTo<DevicePackageViewModel>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
                result.Add(new MostDevicePackageViewModel
                {
                    TotalSold = package.TotalSold,
                    DevicePackage = pac!
                });
            }
            return result;
        }

        public async Task<ListViewModel<DevicePackageViewModel>> GetDevicePackages(DevicePackageFilterModel filter, PaginationRequestModel pagination)
        {
            var query = _packageRepository.GetAll();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(device => device.Name.Contains(filter.Name));
            }

            if (!string.IsNullOrEmpty(filter.ManufacturerName))
            {
                query = query.Where(device => device.Manufacturer.Name.Contains(filter.ManufacturerName));
            }

            if (filter.ManufacturerId.HasValue)
            {
                query = query.Where(device => device.ManufacturerId.Equals(filter.ManufacturerId.Value));
            }
            
            if (!string.IsNullOrEmpty(filter.Status.ToString()))
            {
                query = query.Where(device => device.Status.Equals(filter.Status.ToString()));
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(device => device.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(device => device.Price <= filter.MaxPrice.Value);
            }

            var totalRow = await query.AsNoTracking().CountAsync();
            var paginatedQuery = query
                .OrderByDescending(device => device.CreateAt)
                .Skip(pagination.PageNumber * pagination.PageSize)
                .Take(pagination.PageSize);

            var devicePackages = await paginatedQuery
                .ProjectTo<DevicePackageViewModel>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
            return new ListViewModel<DevicePackageViewModel>
            {
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalRow = totalRow
                },
                Data = devicePackages
            };
        }

        public async Task<DevicePackageDetailViewModel> GetDevicePackage(Guid id)
        {
            return await _packageRepository.GetMany(device => device.Id.Equals(id))
                                            .ProjectTo<DevicePackageDetailViewModel>(_mapper.ConfigurationProvider)
                                            .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy device package");
        }

        public async Task<DevicePackageDetailViewModel> CreateDevicePackage(CreateDevicePackageModel model)
        {
            CheckImage(model.Image);

            var result = 0;
            var devicePackageId = Guid.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    var manufactureId = await CheckManufacturer(model.ManufacturerId);
                    List<Promotion>? promotion = await CheckPromotion(model.PromotionIds) ?? new List<Promotion>();
                    devicePackageId = Guid.NewGuid();
                    var totalPrice = await AddSmartDevices(devicePackageId, model.SmartDeviceIds);

                    var devicePackage = new DevicePackage
                    {
                        Id = devicePackageId,
                        ManufacturerId = manufactureId,
                        Promotions = promotion,
                        Name = model.Name,
                        WarrantyDuration = model.WarrantyDuration,
                        CompletionTime = model.CompletionTime,
                        Description = model.Description,
                        Price = totalPrice,
                        Status = DevicePackageStatus.Active.ToString(),
                    };

                    _packageRepository.Add(devicePackage);

                    await CreateDevicePackageImage(devicePackageId, model.Image, false);

                    result = await _unitOfWork.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };

            return result > 0 ? await GetDevicePackage(devicePackageId) : null!;
        }

        public async Task<DevicePackageDetailViewModel> UpdateDevicePackage(Guid id, UpdateDevicePackageModel model)
        {
            var devicePackage = await _packageRepository.GetMany(device => device.Id.Equals(id))
                .Include(x => x.Promotions)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy device package");

            if (model.ManufacturerId.HasValue)
            {
                var manufacturerId = await CheckManufacturer((Guid)model.ManufacturerId);
                devicePackage.ManufacturerId = manufacturerId;
            }

            if(model.Image != null)
            {
                CheckImage(model.Image);
                await CreateDevicePackageImage(id, model.Image, true);
            }

            if(model.SmartDevices != null && model.SmartDevices.Count > 0)
            {
                var totalPrice = await UpdateSmartDevices(id, model.SmartDevices);
                devicePackage.Price = totalPrice;

                //var (totalPrice, smartDevices) = await GetSmartDevices(model.SmartDevicesIds);
                //devicePackage.SmartDevices.Clear();
                //devicePackage.SmartDevices = smartDevices;
                //devicePackage.Price = totalPrice;
            }

            if(model.PromotionIds != null && model.PromotionIds.Count() > 0)
            {
                var promotion = await CheckPromotion(model.PromotionIds) ?? new List<Promotion>();
                devicePackage.Promotions.Clear();
                devicePackage.Promotions = promotion;
            }

            devicePackage.Name = model.Name ?? devicePackage.Name;
            //devicePackage.PromotionId = model.PromotionId ?? devicePackage.PromotionId;
            devicePackage.WarrantyDuration = model.WarrantyDuration ?? devicePackage.WarrantyDuration;
            devicePackage.CompletionTime = model.CompletionTime ?? devicePackage.CompletionTime;
            devicePackage.Description = model.Description ?? devicePackage.Description;
            devicePackage.Status = model.Status ?? devicePackage.Status;

            _packageRepository.Update(devicePackage);

            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetDevicePackage(id) : null!;
        }

        //PRIVATE METHOD

        private async Task<Image> CreateDevicePackageImage(Guid devicePackageId, IFormFile image, bool isUpdate)
        {
            if (isUpdate)
            {
                var listImages = await _imageRepository.GetMany(image => image.DevicePackageId.Equals(devicePackageId)).ToListAsync();
                foreach(var img in listImages)
                {
                    await _cloudStorageService.Delete(img.Id);
                }
                _imageRepository.RemoveRange(listImages);
            }

            var imageId = Guid.NewGuid();
            var url = await _cloudStorageService.Upload(imageId, image.ContentType, image.OpenReadStream());
            var newImage = new Image
            {
                Id = imageId,
                DevicePackageId = devicePackageId,
                Url = url
            };

            _imageRepository.Add(newImage);
            return newImage;
        }

        private void CheckImage(IFormFile image)
        {
            if (!image.ContentType.StartsWith("image/"))
            {
                throw new BadRequestException("File không phải là hình ảnh");
            }
        }

        private async Task<Guid> CheckManufacturer(Guid id)
        {
            return (await _manufacturerRepository
                    .GetMany(manufacturer => manufacturer.Id.Equals(id))
                    .FirstOrDefaultAsync())?.Id ?? throw new NotFoundException("Không tìm thấy manufacturer");
        }

        private async Task<List<Promotion>?> CheckPromotion(List<Guid>? promotionIds)
        {
            if (promotionIds != null && promotionIds.Count > 0)
            {
                var uniqueIds = new HashSet<Guid>();
                var result = new List<Promotion>();
                
                foreach(var id in promotionIds) { 
                    if(uniqueIds.Add(id))
                    {
                        var pro = await _promotionRepository.GetMany(p => p.Id == id && p.Status == PromotionStatus.Active.ToString())
                       .FirstOrDefaultAsync() ?? throw new BadRequestException("Promotion không tồn tại hoặc đã hết hạn");

                        result.Add(pro);
                    }
                }

                var flag = result.Sum(s => s.DiscountAmount);
                if(flag >= 100)
                {
                    throw new BadRequestException("Total discount của sản phẩm quá 100%");
                }

                return result;
            }
            return null;
        }

        private async Task<int> AddSmartDevices(Guid packageId, List<SmartDevices> smartDevices)
        {
            int totalPrice = 0;

            var uniques = new HashSet<Guid>();
            foreach(var item in smartDevices)
            {
                if (uniques.Add(item.SmartDeviceId))
                {
                    var device = await _deviceRepository.GetMany(device => device.Id.Equals(item.SmartDeviceId))
                    .FirstOrDefaultAsync() ?? throw new NotFoundException($"Không tìm thấy smart device với id: {item.SmartDeviceId}");

                var addDeviceToPackage = new SmartDevicePackage
                {
                    SmartDeviceId = device.Id,
                    DevicePackageId = packageId,
                    SmartDeviceQuantity = item.Quantity.GetValidOrDefault(1)
                };

                    _smartDevicePackage.Add(addDeviceToPackage);

                    totalPrice += device.Price * item.Quantity;
                }
                 
            }
            return totalPrice;
        }

        private async Task<int> UpdateSmartDevices(Guid packageId, List<SmartDevices> smartDevices)
        {
            var totalPrice = 0;

            var existingSmartDevicePackages = await _smartDevicePackage
                .GetMany(sdp => sdp.DevicePackageId == packageId)
                .ToListAsync();

            var smartDevicesToUpdate = existingSmartDevicePackages
                .Where(sdp => smartDevices.Any(usd => usd.SmartDeviceId == sdp.SmartDeviceId))
                .ToList();

            var smartDevicesToRemove = existingSmartDevicePackages
                .Except(smartDevicesToUpdate)
                .ToList();
            _smartDevicePackage.RemoveRange(smartDevicesToRemove);


            foreach (var updatedSmartDevice in smartDevices)
            {
                var device = await _deviceRepository.GetMany(device => device.Id.Equals(updatedSmartDevice.SmartDeviceId))
                    .FirstOrDefaultAsync() ?? throw new NotFoundException($"Không tìm thấy smart device với id: {updatedSmartDevice.SmartDeviceId}");
                var quantity = updatedSmartDevice.Quantity.GetValidOrDefault(1);

                var existingSmartDevicePackage = smartDevicesToUpdate.FirstOrDefault(sdp => sdp.SmartDeviceId == device.Id);

                if (existingSmartDevicePackage != null)
                {
                    existingSmartDevicePackage.SmartDeviceQuantity = updatedSmartDevice.Quantity.GetValidOrDefault(1);
                }
                else
                {
                    _smartDevicePackage.Add(new SmartDevicePackage
                    {
                        SmartDeviceId = device.Id,
                        DevicePackageId = packageId,
                        SmartDeviceQuantity = quantity
                    });
                }

                totalPrice += device.Price * quantity;
            }

            return totalPrice;
        }

    }
}
