using AutoMapper;
using ISHE_Data.Entities;
using ISHE_Data.Models.Views;

namespace ISHE_Data.Mapping
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<Role, RoleViewModel>();

            CreateMap<Account, AccountViewModel>()
                .ForMember(dest => dest.RoleName, otp => otp.MapFrom(acc => acc.Role.RoleName))
                .ForMember(dest => dest.FullName, otp => otp.MapFrom(acc => acc.OwnerAccount != null ? acc.OwnerAccount.FullName :
                                                         (acc.StaffAccount != null ? acc.StaffAccount.FullName :
                                                         (acc.TellerAccount != null ? acc.TellerAccount.FullName :
                                                         (acc.CustomerAccount != null ? acc.CustomerAccount.FullName : string.Empty)))))
                .ForMember(dest => dest.Email, otp => otp.MapFrom(acc => acc.OwnerAccount != null ? acc.OwnerAccount.Email :
                                                         (acc.StaffAccount != null ? acc.StaffAccount.Email :
                                                         (acc.TellerAccount != null ? acc.TellerAccount.Email :
                                                         (acc.CustomerAccount != null ? acc.CustomerAccount.Email : string.Empty)))))
                .ForMember(dest => dest.Avatar, otp => otp.MapFrom(acc => acc.OwnerAccount != null ? acc.OwnerAccount.Avatar :
                                         (acc.StaffAccount != null ? acc.StaffAccount.Avatar :
                                         (acc.TellerAccount != null ? acc.TellerAccount.Avatar :
                                         (acc.CustomerAccount != null ? acc.CustomerAccount.Avatar : string.Empty)))));

            CreateMap<OwnerAccount, OwnerViewModel>()
                .ForMember(dest => dest.PhoneNumber, otp => otp.MapFrom(acc => acc.Account.PhoneNumber))
                .ForMember(dest => dest.RoleName, otp => otp.MapFrom(acc => acc.Account.Role.RoleName))
                .ForMember(dest => dest.Status, otp => otp.MapFrom(acc => acc.Account.Status))
                .ForMember(dest => dest.CreateAt, otp => otp.MapFrom(acc => acc.Account.CreateAt));

            CreateMap<StaffAccount, StaffViewModel>()
                .ForMember(dest => dest.PhoneNumber, otp => otp.MapFrom(acc => acc.Account.PhoneNumber))
                .ForMember(dest => dest.RoleName, otp => otp.MapFrom(acc => acc.Account.Role.RoleName))
                .ForMember(dest => dest.Status, otp => otp.MapFrom(acc => acc.Account.Status))
                .ForMember(dest => dest.CreateAt, otp => otp.MapFrom(acc => acc.Account.CreateAt));

            CreateMap<StaffAccount, StaffGroupViewModel>()
                .ForMember(dest => dest.LeadAccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.LeadPhoneNumber, opt => opt.MapFrom(src => src.Account.PhoneNumber))
                .ForMember(dest => dest.LeadFullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.LeadEmail, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.LeadAvatar, opt => opt.MapFrom(src => src.Avatar))
                .ForMember(dest => dest.LeadIsLead, opt => opt.MapFrom(src => src.IsLead))
                .ForMember(dest => dest.StaffMembers, opt => opt.MapFrom(src => src.InverseStaffLead));

            CreateMap<CustomerAccount, CustomerViewModel>()
                .ForMember(dest => dest.PhoneNumber, otp => otp.MapFrom(acc => acc.Account.PhoneNumber))
                .ForMember(dest => dest.RoleName, otp => otp.MapFrom(acc => acc.Account.Role.RoleName))
                .ForMember(dest => dest.Status, otp => otp.MapFrom(acc => acc.Account.Status))
                .ForMember(dest => dest.CreateAt, otp => otp.MapFrom(acc => acc.Account.CreateAt));

            CreateMap<Image, ImageViewModel>();
            CreateMap<Manufacturer, ManufacturerViewModel>();
            CreateMap<SmartDevice, SmartDeviceDetailViewModel>();
            CreateMap<SmartDevice, SmartDeviceViewModel>()
                .ForMember(dest => dest.Image, otp => otp.MapFrom(src => src.Images.FirstOrDefault()!.Url))
                .ForMember(dest => dest.Manufacturer, otp => otp.MapFrom(src => src.Manufacturer.Name));
            CreateMap<Promotion, PromotionViewModel>();
            CreateMap<Promotion, PromotionDetailViewModel>();
            CreateMap<DevicePackage, DevicePackageViewModel>();
            CreateMap<DevicePackage, DevicePackageDetailViewModel>();
            CreateMap<SurveyRequest, SurveyRequestViewModel>();
            CreateMap<Survey, SurveyViewModel>();
            CreateMap<Contract, PartialContractViewModel>();
            CreateMap<Contract, ContractViewModel>();
            CreateMap<ContractDetail, ContractDetailViewModel>()
                .ForMember(dest => dest.Image, otp => otp.MapFrom(src => src.SmartDevice.Images.FirstOrDefault()!.Url))
                .ForMember(dest => dest.Manufacturer, otp => otp.MapFrom(src => src.SmartDevice.Manufacturer.Name));
            CreateMap<DevicePackageUsage, DevicePackageUsageViewModel>()
                .ForMember(dest => dest.Name, otp => otp.MapFrom(src => src.DevicePackage.Name))
                .ForMember(dest => dest.Manufacturer, otp => otp.MapFrom(src => src.DevicePackage.Manufacturer.Name))
                .ForMember(dest => dest.Image, otp => otp.MapFrom(src => src.DevicePackage.Images.FirstOrDefault()!.Url));
            CreateMap<Payment, PaymentViewModel>();
            CreateMap<SmartDevicePackage, SmartDevicePackageViewModel>();
            CreateMap<TellerAccount, TellerViewModel>()
                .ForMember(dest => dest.PhoneNumber, otp => otp.MapFrom(src => src.Account.PhoneNumber))
                .ForMember(dest => dest.RoleName, otp => otp.MapFrom(src => src.Account.Role.RoleName))
                .ForMember(dest => dest.Status, otp => otp.MapFrom(src => src.Account.Status))
                .ForMember(dest => dest.CreateAt, otp => otp.MapFrom(acc => acc.Account.CreateAt));
            CreateMap<CustomerAccount, PartialCustomerViewModel>()
                .ForMember(dest => dest.PhoneNumber, otp => otp.MapFrom(src => src.Account.PhoneNumber));
            CreateMap<Acceptance, AcceptanceViewModel>();
            CreateMap<FeedbackDevicePackage, FeedbackDevicePackageViewModel>();
            CreateMap<Survey, PartialSurveyViewModel>();
            CreateMap<Notification, NotificationViewModel>()
                .ForMember(notificationVM => notificationVM.Data, config => config.MapFrom(notification => new NotificationDataViewModel
                {
                    CreateAt = notification.CreateAt,
                    IsRead = notification.IsRead,
                    Link = notification.Link,
                    Type = notification.Type
                }));
            CreateMap<ContractModificationRequest, ContractModificationViewModel>();
        }
    }
}
