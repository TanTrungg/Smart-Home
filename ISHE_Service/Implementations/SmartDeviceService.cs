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
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ISHE_Service.Implementations
{
    public class SmartDeviceService : BaseService, ISmartDeviceService
    {
        private readonly ISmartDeviceRepository _smartDeviceRepository;
        private readonly IDevicePackageRepository _devicePackageRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IManufacturerRepository _manufacturerRepository;

        private readonly ICloudStorageService _cloudStorageService;
        public SmartDeviceService(IUnitOfWork unitOfWork, IMapper mapper, ICloudStorageService cloudStorageService) : base(unitOfWork, mapper)
        {
            _smartDeviceRepository = unitOfWork.SmartDevice;
            _imageRepository = unitOfWork.Image;
            _manufacturerRepository = unitOfWork.Manufacturer;
            _cloudStorageService = cloudStorageService;
            _devicePackageRepository = unitOfWork.DevicePackage;
        }

        public async Task<ListViewModel<SmartDeviceDetailViewModel>> GetSmartDevices(SmartDeviceFilterModel filter, PaginationRequestModel pagination)
        {
            var query = _smartDeviceRepository.GetAll();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(device => device.Name.Contains(filter.Name));
            }

            if (!string.IsNullOrEmpty(filter.DeviceType))
            {
                query = query.Where(device => device.DeviceType!.Contains(filter.DeviceType));
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

            var smartDevices = await paginatedQuery
                .ProjectTo<SmartDeviceDetailViewModel>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
            return new ListViewModel<SmartDeviceDetailViewModel>
            {
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalRow = totalRow
                },
                Data = smartDevices
            };
        }

        public async Task<SmartDeviceDetailViewModel> GetSmartDevice(Guid id)
        {
            return await _smartDeviceRepository.GetMany(device => device.Id.Equals(id))
                .ProjectTo<SmartDeviceDetailViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy smart device");
        }

        public async Task<SmartDeviceDetailViewModel> CreateSmartDevice(CreateSmartDeviceModel model)
        {
            CheckImage(model.Images);

            var result = 0;
            var smartDeviceId = Guid.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    var manufacturerId = await CheckManufacturer(model.ManufacturerId);
                    smartDeviceId = Guid.NewGuid();
                    var smartDevice = new SmartDevice
                    {
                        Id = smartDeviceId,
                        ManufacturerId = manufacturerId,
                        Name = model.Name,
                        Description = model.Description,
                        Price = model.Price,
                        InstallationPrice = model.InstallationPrice,
                        DeviceType = model.DeviceType,
                        Status = SmartDeviceStatus.Active.ToString(),
                    };

                    _smartDeviceRepository.Add(smartDevice);

                    await CreateSmartDeviceImage(smartDeviceId, model.Images);

                    result = await _unitOfWork.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };
            return result > 0 ? await GetSmartDevice(smartDeviceId) : null!;
        }

        public async Task<SmartDeviceDetailViewModel> UpdateSmartDevice(Guid id, UpdateSmartDeviceModel model)
        {
            
            var smartDevice = await _smartDeviceRepository.GetMany(device => device.Id.Equals(id))
                                    .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy smart device");

            if (model.ManufacturerId.HasValue)
            {
                var manufactureId = await CheckManufacturer((Guid)model.ManufacturerId);
                smartDevice.ManufacturerId = manufactureId;
            }

            smartDevice.Name = model.Name ?? smartDevice.Name;
            smartDevice.Description = model.Description ?? smartDevice.Description;
            smartDevice.Price = model.Price ?? smartDevice.Price;
            smartDevice.InstallationPrice = model.InstallationPrice ?? smartDevice.InstallationPrice;
            smartDevice.DeviceType = model.DeviceType ?? smartDevice.DeviceType;

            if (!string.IsNullOrEmpty(model.Status))
            {
                await UpdateStatus(model.Status, smartDevice);
            }

            _smartDeviceRepository.Update(smartDevice);

            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetSmartDevice(id) : null!;
        }

        private async Task UpdateStatus(string newStatus, SmartDevice smartDevice)
        {
            if (newStatus != smartDevice.Status)
            {
                if(newStatus == SmartDeviceStatus.Active.ToString() || newStatus == SmartDeviceStatus.InActive.ToString())
                {
                    var packages = await _devicePackageRepository.GetMany(d => d.SmartDevicePackages.Any(x => x.SmartDeviceId == smartDevice.Id)).ToListAsync();
                    foreach (var package in packages)
                    {
                        if (newStatus == nameof(SmartDeviceStatus.InActive))
                        {
                            package.Price -= smartDevice.Price;
                        }
                        else if (newStatus == nameof(SmartDeviceStatus.Active))
                        {
                            package.Price += smartDevice.Price;
                        }
                        package.Status = package.Price > 0 ? DevicePackageStatus.Active.ToString() : DevicePackageStatus.InActive.ToString();
                    }
                    _devicePackageRepository.UpdateRange(packages);

                    smartDevice.Status = newStatus;
                }
                else
                {
                    throw new BadRequestException($"Không thể cập nhập trạng thái từ {smartDevice.Status} thành {newStatus}");
                }
                
            }
        }

        public async Task<SmartDeviceDetailViewModel> UpdateSmartDeviceImage(Guid id, UpdateImageModel model)
        {
            CheckImage(model.Images);

            var smartDevice = await _smartDeviceRepository.GetMany(device => device.Id.Equals(id))
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy smart device");

            foreach (var image in model.Images)
            {
                var imageId = Guid.NewGuid();
                var url = await _cloudStorageService.Upload(imageId, image.ContentType, image.OpenReadStream());
                var newImage = new Image
                {
                    Id = imageId,
                    SmartDeviceId = id,
                    Url = url
                };
                _imageRepository.Add(newImage);
            }
            return await _unitOfWork.SaveChanges() > 0 ? await GetSmartDevice(id) : null!;
        }

        public async Task RemoveSmartDeviceImage(List<Guid> imageIds)
        {
            if (imageIds.Count != 0)
            {
                foreach (var imageId in imageIds)
                {
                    var existImage = await _imageRepository.GetMany(image => image.Id.Equals(imageId)).FirstOrDefaultAsync();
                    if (existImage != null)
                    {
                        await _cloudStorageService.Delete(imageId);
                        _imageRepository.Remove(existImage);

                        await _unitOfWork.SaveChanges();
                    }
                }
            }
        }


        //PRIVATE METHOD
        private async Task<ICollection<Image>> CreateSmartDeviceImage(Guid id, ICollection<IFormFile> images)
        {
            var listImage = new List<Image>();
            foreach (IFormFile image in images)
            {
                var imageId = Guid.NewGuid();
                var url = await _cloudStorageService.Upload(imageId, image.ContentType, image.OpenReadStream());
                var newImage = new Image
                {
                    Id = imageId,
                    SmartDeviceId = id,
                    Url = url
                };
                listImage.Add(newImage);
            }
            _imageRepository.AddRange(listImage);
            return listImage;
        }


        private void CheckImage(List<IFormFile> images)
        {
            var imageCount = images.Count();

            if (imageCount < 1 || imageCount > 4)
            {
                throw new BadRequestException("Phải có ít nhất một hình để tạo và không được quá 4 hình.");
            }

            foreach (IFormFile image in images)
            {
                if (!image.ContentType.StartsWith("image/"))
                {
                    throw new BadRequestException("File không phải là hình ảnh");
                }
            }
        }

        private async Task<Guid> CheckManufacturer(Guid id)
        {
            return (await _manufacturerRepository
                    .GetMany(manufacturer => manufacturer.Id.Equals(id))
                    .FirstOrDefaultAsync())?.Id ?? throw new NotFoundException("Không tìm thấy manufacturer");
        }
    }
}
