using AutoMapper;
using AutoMapper.QueryableExtensions;
using ISHE_Data;
using ISHE_Data.Entities;
using ISHE_Data.Models.Requests.Filters;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Views;
using ISHE_Data.Repositories.Interfaces;
using ISHE_Service.Interfaces;
using ISHE_Utility.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ISHE_Service.Implementations
{
    public class ManufacturerService : BaseService, IManufacturerService
    {
        private readonly IManufacturerRepository _manufacturerRepository;
        public ManufacturerService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _manufacturerRepository = unitOfWork.Manufacturer;
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

            var manufacturer = new Manufacturer
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
            };

            _manufacturerRepository.Add(manufacturer);

            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetManufacturer(manufacturer.Id) : null!;
        }

        public async Task<ManufacturerViewModel> UpdateManufacturer(Guid id, CreateManufacturerModel model)
        {
            var manufacturer = await _manufacturerRepository
                .GetMany(manufac => manufac.Id.Equals(id))
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy manufacturer");

            manufacturer.Name = model.Name ?? manufacturer.Name;

            _manufacturerRepository.Update(manufacturer);

            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetManufacturer(manufacturer.Id) : null!;
        }
    }
}
