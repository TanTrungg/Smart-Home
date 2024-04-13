using AutoMapper;
using AutoMapper.QueryableExtensions;
using ISHE_Data;
using ISHE_Data.Entities;
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
    public class ContractModificationService : BaseService, IContractModificationService
    {
        private readonly string[] validTypes = { "Modify", "Cancel" };
        private readonly IContractModificationRepository _contractModification;
        private readonly IContractRepository _contractRepository;
        public ContractModificationService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _contractModification = unitOfWork.ContractModification;
            _contractRepository = unitOfWork.Contract;
        }

        public async Task<ContractModificationViewModel> GetContractModification(Guid id)
        {
            return await _contractModification.GetMany(ct => ct.Id == id)
                .ProjectTo<ContractModificationViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy contract modification");
        }

        public async Task<ContractModificationViewModel> CreateContractModification(CreateContractModificationModel model)
        {
            await ValidateType(model.Type, model.ContractId);
            var id = Guid.NewGuid();
            var contractModification = new ContractModificationRequest
            {
                Id = id,
                ContractId = model.ContractId,
                Type = model.Type,
                Description = model.Description,
                Status = ContractModificationStatus.Pending.ToString()
            };

            _contractModification.Add(contractModification);

            var result = await _unitOfWork.SaveChanges();

            return result > 0 ? await GetContractModification(id) : null!;
        }

        public async Task<ContractModificationViewModel> UpdateContractModification(Guid id, UpdateContractModificationModel model)
        {
            var contractModification = await _contractModification.GetMany(ct => ct.Id == id).FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy contract modification");

            if (!string.IsNullOrEmpty(model.Status))
            {
                UpdateStatus(contractModification, model.Status);
            }

            contractModification.Description = model.Description ?? contractModification.Description;

            _contractModification.Update(contractModification);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetContractModification(id) : null!;
        }

        private async Task ValidateType(string type, string contractId)
        {
            if (!validTypes.Contains(type))
            {
                throw new BadRequestException("Type không hợp lệ. Vui lòng chọn Type từ danh sách sau: " + string.Join(", ", validTypes));
            }

            var flag = await _contractRepository.GetMany(c => c.Id == contractId).FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy contract");
        }

        private void UpdateStatus(ContractModificationRequest contractModification, string newStatus)
        {
            switch (contractModification.Status)
            {
                case nameof(ContractModificationStatus.Pending):
                    if(newStatus == nameof(ContractModificationStatus.Approved) || newStatus == nameof(ContractModificationStatus.Rejected))
                    {
                        contractModification.Status = newStatus;
                    }
                    else
                    {
                        throw new BadRequestException($"Không thể cập nhật trạng thái từ {contractModification.Status} thành {newStatus}");
                    }
                    break;
                case nameof(ContractModificationStatus.Approved):
                case nameof(ContractModificationStatus.Rejected):
                    break;
                    //throw new BadRequestException($"Không thể cập nhật trạng thái từ {contractModification.Status} thành {newStatus}");
                default:
                    break;
            }
        }
    }
}
