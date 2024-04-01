using ISHE_Data.Entities;
using ISHE_Data.Repositories.Implementations;
using ISHE_Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace ISHE_Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SMART_HOME_DBContext _context;

        private IAcceptanceRepository _acceptance = null!;
        private IAccountRepository _account = null!;
        private ICustomerAccountRepository _customerAccount = null!;
        private IOwnerAccountRepository _ownerAccount = null!;
        private IRoleRepository _role = null!;
        private IStaffAccountRepository _staffAccount = null!;
        private ITellerAccountRepository _tellerAccount = null!;
        private ISmartDeviceRepository _smartDevice = null!;
        private IImageRepository _image = null!;
        private IManufacturerRepository _manufacturer = null!;
        private IPromotionRepository _promotion = null!;
        private IDevicePackageRepository _devicePackage = null!;
        private ISurveyRequestRepository _surveyRequest = null!;
        private ISurveyRepository _survey = null!;
        private IContractRepository _contract = null!;
        private IContractDetailRepository _contractDetail = null!;
        private IDevicePackageUsageRepository _devicePackageUsage = null!;
        private IPaymentRepository _payment = null!;
        private ISmartDevicePackageRepository _smartDevicePackage = null!;
        private IFeedbackDevicePackageRepository _feedbackDevicePackage = null!;
        private INotificationRepository _notification = null!;
        private IDeviceTokenRepository _deviceToken = null!;
        private IContractModificationRepository _contractModification = null!;

        public UnitOfWork(SMART_HOME_DBContext context)
        {
            _context = context;
        }

        public IAcceptanceRepository Acceptance
        {
            get { return _acceptance ??= new AcceptanceRepository(_context); }
        }

        public IAccountRepository Account
        {
            get { return _account ??= new AccountRepository(_context); }
        }

        public ICustomerAccountRepository CustomerAccount
        {
            get { return _customerAccount ??= new CustomerAccountRepository(_context); }
        }

        public IOwnerAccountRepository OwnerAccount
        {
            get { return _ownerAccount ??= new OwnerAccountRepository(_context); }
        }

        public IRoleRepository Role
        {
            get { return _role ??= new RoleRepository(_context); }
        }

        public IStaffAccountRepository StaffAccount
        {
            get { return _staffAccount ??= new StaffAccountRepository(_context); }
        }

        public ITellerAccountRepository TellerAccount
        {
            get { return _tellerAccount ??= new TellerAccountRepository(_context); }
        }

        public ISmartDeviceRepository SmartDevice
        {
            get { return _smartDevice ??= new SmartDeviceRepository(_context); }
        }

        public IImageRepository Image
        {
            get { return _image ??= new ImageRepository(_context); }
        }

        public IManufacturerRepository Manufacturer
        {
            get { return _manufacturer ??= new ManufacturerRepository(_context); }
        }

        public IPromotionRepository Promotion
        {
            get { return _promotion ??= new PromotionRepository(_context); }
        }
        public IDevicePackageRepository DevicePackage
        {
            get { return _devicePackage ??= new DevicePackageRepository(_context); }
        }

        public ISurveyRequestRepository SurveyRequest
        {
            get { return _surveyRequest ??= new SurveyRequestRepository(_context); }
        }

        public ISurveyRepository Survey
        {
            get { return _survey ??= new SurveyRepository(_context); }
        }

        public IContractRepository Contract
        {
            get { return _contract ??= new ContractRepository(_context); }
        }
        public IContractDetailRepository ContractDetail
        {
            get { return _contractDetail ??= new ContractDetailRepository(_context); }
        }
        public IDevicePackageUsageRepository DevicePackageUsage
        {
            get { return _devicePackageUsage ??= new DevicePackageUsageRepository(_context); }
        }
        public IPaymentRepository Payment
        {
            get { return _payment ??= new PaymentRepository(_context); }
        }

        public ISmartDevicePackageRepository SmartDevicePackage
        {
            get { return _smartDevicePackage ??= new SmartDevicePackageRepository(_context); }
        }

        public IFeedbackDevicePackageRepository FeedbackDevicePackage
        {
            get { return _feedbackDevicePackage ??= new FeedbackDevicePackageRepository(_context); }
        }

        public INotificationRepository Notification
        {
            get { return _notification ??= new NotificationRepository(_context); }
        }

        public IDeviceTokenRepository DeviceToken
        {
            get { return _deviceToken ??= new DeviceTokenRepository(_context); }
        }

        public IContractModificationRepository ContractModification
        {
            get { return _contractModification ??= new ContractModificationRepository(_context); }
        }

        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }

        public IDbContextTransaction Transaction()
        {
            return _context.Database.BeginTransaction();
        }
    }
}
