using AutoMapper;
using AutoMapper.QueryableExtensions;
using ISHE_Data;
using ISHE_Data.Entities;
using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Requests.Put;
using ISHE_Data.Models.Views;
using ISHE_Data.Repositories.Interfaces;
using ISHE_Service.Interfaces;
using ISHE_Utility.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ISHE_Service.Implementations
{
    public class ManufacturerService : BaseService, IManufacturerService
    {
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly ICloudStorageService _cloudStorageService;
        public ManufacturerService(IUnitOfWork unitOfWork, IMapper mapper, ICloudStorageService cloudStorageService) : base(unitOfWork, mapper)
        {
            _manufacturerRepository = unitOfWork.Manufacturer;
            _cloudStorageService = cloudStorageService;
        }

        public async Task<List<ManufacturerViewModel>> GetManufacturers(ManufacturerFilterModel filter)
        {
            var query = _manufacturerRepository.GetAll();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(manufacturer => manufacturer.Name.Contains(filter.Name));
            }

            return await query.ProjectTo<ManufacturerViewModel>(_mapper.ConfigurationProvider)
                                .OrderBy(manufacturer => manufacturer.Name)
                                .ToListAsync();
        }

        public async Task<ManufacturerViewModel> GetManufacturer(Guid id)
        {
            return await _manufacturerRepository.GetMany(manufacturer => manufacturer.Id.Equals(id))
                                                .ProjectTo<ManufacturerViewModel>(_mapper.ConfigurationProvider)
                                                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy manufacturer");
        }

        public async Task<ManufacturerViewModel> CreateManufacturer(CreateManufacturerModel model)
        {
            if(_manufacturerRepository.Any(manufacturer => manufacturer.Name.Equals(model.Name)))
            {
                throw new ConflictException("Manufacturer đã tồn tại");
            }
            var id = Guid.NewGuid();
            var imageUrl = await UploadImage(id, model.Image, false);
            var manufacturer = new Manufacturer
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Origin = model.Origin,
                Description = model.Description,
                Image = imageUrl
            };

            _manufacturerRepository.Add(manufacturer);

            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetManufacturer(manufacturer.Id) : null!;
        }

        public async Task<ManufacturerViewModel> UpdateManufacturer(Guid id, UpdateManufacturerModel model)
        {
            var manufacturer = await _manufacturerRepository
                .GetMany(manufac => manufac.Id.Equals(id))
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy manufacturer");

            if(model.Image != null)
            {
                manufacturer.Image = await UploadImage(id, model.Image, true);
            }

            manufacturer.Name = model.Name ?? manufacturer.Name;
            manufacturer.Origin = model.Origin ?? manufacturer.Origin;
            manufacturer.Description = model.Description ?? manufacturer.Description;


            _manufacturerRepository.Update(manufacturer);

            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetManufacturer(manufacturer.Id) : null!;
        }

        //PRIVATE METHOD
        private async Task<string> UploadImage(Guid id, IFormFile image, bool isUpdate)
        {
            if (!image.ContentType.StartsWith("image/"))
            {
                throw new BadRequestException("File không phải là hình ảnh");
            }

            if(isUpdate)
            {
                await _cloudStorageService.Delete(id);
            }

            var url = await _cloudStorageService.Upload(id, image.ContentType, image.OpenReadStream());
            return url;
        }
    }
}
