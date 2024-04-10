using AutoMapper;
using AutoMapper.QueryableExtensions;
using ISHE_Data;
using ISHE_Data.Entities;
using ISHE_Data.Models.Requests.Post;
using ISHE_Data.Models.Views;
using ISHE_Data.Repositories.Implementations;
using ISHE_Data.Repositories.Interfaces;
using ISHE_Service.Interfaces;
using ISHE_Utility.Constants;
using ISHE_Utility.Enum;
using ISHE_Utility.Exceptions;
using ISHE_Utility.Helpers.ZaloPay;
using ISHE_Utility.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ISHE_Service.Implementations
{
    public class PaymentService : BaseService, IPaymentService
    {
        private readonly IContractRepository _contract;
        private readonly IPaymentRepository _payment;
        private readonly INotificationService _notificationService;
        private readonly AppSetting _appSettings;
        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<AppSetting> appSettings, INotificationService notificationService) : base(unitOfWork, mapper)
        {
            _contract = unitOfWork.Contract;
            _payment = unitOfWork.Payment;
            _appSettings = appSettings.Value;
            _notificationService = notificationService;
        }

        public async Task ProcessCashPayment(CreatePaymentModel model)
        {
            var existingPayment = await _payment.GetMany(payment => payment.ContractId.Equals(model.ContractId)
                                                           && payment.Name.Equals(model.TypePayment)).FirstOrDefaultAsync();
            if (existingPayment != null)
            {
                return;
            }

            var contract = await _contract.GetMany(ct => ct.Id.Equals(model.ContractId))
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy contract");
            var totalAmount = HandleTotalAmountOfPayment(contract, model.TypePayment);

            var payment = new Payment
            {
                Id = DateTime.Now.ToString("yyMMddHHmmssfff") + "_" + model.ContractId,
                ContractId = model.ContractId,
                Name = model.TypePayment,
                PaymentMethod = PaymentMethod.Cash,
                Amount = totalAmount,
                Status = PaymentStatus.Successful.ToString()
            };

            _payment.Add(payment);
            //await _unitOfWork.SaveChanges();
        }

        public async Task<dynamic> ProcessZalopayPayment(CreatePaymentModel model)
        {
            var existingPayment = await _payment.GetMany(payment => payment.ContractId.Equals(model.ContractId)
                                                           && payment.Name.Equals(model.TypePayment)).FirstOrDefaultAsync();
            if (existingPayment != null)
            {
                //return Task.CompletedTask;
                _payment.Remove(existingPayment);
            }

            var contract = await _contract.GetMany(ct => ct.Id.Equals(model.ContractId))
                .Include(contract => contract.Customer)
                    .ThenInclude(customer => customer.Account)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy contract");

            var customerPhone = contract.Customer.Account.PhoneNumber;
            var totalAmount = HandleTotalAmountOfPayment(contract, model.TypePayment);

            //item?
            var items = new[] { new { } };

            var embed_data = new
            {
                merchantinfo = "Phat Dat Store",
                preferred_payment_method = new object[] { },
                redirecturl = $"https://www.youtube.com"
            };
            DateTime now = DateTime.UtcNow.AddHours(7);
            string AppTransId = now.ToString("yyMMddHHmmssfff") + "_" + model.ContractId;

            var payment = new Payment
            {
                Id = AppTransId,
                ContractId = model.ContractId,
                Name = model.TypePayment,
                PaymentMethod = PaymentMethod.Zalopay,
                Amount = totalAmount,
                Status = PaymentStatus.Pending.ToString()
            };
            _payment.Add(payment);
            await _unitOfWork.SaveChanges();

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long unixTime = (long)(DateTime.UtcNow - epoch).TotalMilliseconds;

            var param = new Dictionary<string, string>
                {
                { "app_id", _appSettings.ZaloPay.ZaloPayAppId.ToString() },
                { "app_user", customerPhone },
                { "app_time", unixTime.ToString() },
                { "amount", totalAmount.ToString() },
                { "app_trans_id", AppTransId },
                { "embed_data", JsonConvert.SerializeObject(embed_data) },
                { "item", JsonConvert.SerializeObject(items) },
                { "description", $"Phat Dat - Thanh toán {model.TypePayment.ToLower()} hợp đồng #{model.ContractId}" },
                { "bank_code", "" },
                { "callback_url", _appSettings.ZaloPay.CallbackUrl }
            };

            var data = string.Join("|", _appSettings.ZaloPay.ZaloPayAppId, param["app_trans_id"], param["app_user"], param["amount"],
                                    param["app_time"], param["embed_data"], param["item"]);
            param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, _appSettings.ZaloPay.ZaloPayKey1, data));
            //Console.WriteLine(JsonConvert.SerializeObject(param, Formatting.Indented));
            var result = await HttpHelper.PostFormAsync(_appSettings.ZaloPay.CreateOrderUrl, param);
            return result;
        }

        public async Task<dynamic> IsValidCallback(dynamic cbdata)
        {
            var result = new Dictionary<string, object>();

            try
            {

                var dataStr = Convert.ToString(cbdata["data"]);
                var reqMac = Convert.ToString(cbdata["mac"]);

                // Parse the nested JSON string
                var dataJson = JsonConvert.DeserializeObject<dynamic>(dataStr);
                var appTransId = Convert.ToString(dataJson["app_trans_id"]);

                var mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, _appSettings.ZaloPay.ZaloPayKey2, dataStr);

                Console.WriteLine("mac = {0}", mac);

                // kiểm tra callback hợp lệ (đến từ ZaloPay server)
                if (!reqMac.Equals(mac))
                {
                    //Console.WriteLine("Lỗi");
                    // callback không hợp lệ
                    result["return_code"] = -1;
                    result["return_message"] = "mac not equal";
                }
                else
                {
                    string id = appTransId;
                    var payment = await _payment.GetMany(tran => tran.Id.Equals(id))
                        .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy thông tin của payment");

                    var contract = await _contract.GetMany(ct => ct.Id.Equals(payment.ContractId)).FirstOrDefaultAsync();
                    if (contract == null) return false;


                    contract.Status = await HandleCheckStatusContract(payment, contract.TellerId);
                    
                    payment.Status = PaymentStatus.Successful.ToString();
                    _contract.Update(contract);

                    _payment.Update(payment);
                    await _unitOfWork.SaveChanges();

                    result["return_code"] = 1;
                    result["return_message"] = "success";
                }
            }
            catch (Exception ex)
            {
                result["return_code"] = 0; // ZaloPay server sẽ callback lại (tối đa 3 lần)
                result["return_message"] = ex.Message;
            }
            return result;
        }

        public async Task<List<PaymentViewModel>> GetRevenues(int? month)
        {
            var query = _payment.GetMany(re => re.Status == PaymentStatus.Successful.ToString());

            if (month.HasValue)
            {
                query = query.Where(re => re.CreateAt.Month == month.Value);
            }

            return await query
                .ProjectTo<PaymentViewModel>(_mapper.ConfigurationProvider)
                .OrderByDescending(re => re.CreateAt)
                .ToListAsync();
        }

        //PRIVATE METHOD

        private async Task<string> HandleCheckStatusContract(Payment payment, Guid tellerId)
        {
            var result = "";
            if (payment.Name.Equals(PaymentType.Deposit))
            {
                result = ContractStatus.DepositPaid.ToString();
                await SendNotificationToTeller(payment.ContractId, "đật cọc", tellerId);
            }
            if (payment.Name.Equals(PaymentType.Completion))
            {
                result = ContractStatus.Completed.ToString();
                await SendNotificationToTeller(payment.ContractId, "hoàn thành", tellerId);
            }
            return result;
        }

        private int HandleTotalAmountOfPayment(Contract contract, string typePayment)
        {
            int totalAmount;

            if (typePayment.Equals(PaymentType.Deposit))
            {
                totalAmount = contract.TotalAmount * contract.Deposit / 100;
            }else if (typePayment.Equals(PaymentType.Completion))
            {
                totalAmount = contract.TotalAmount * (100 - contract.Deposit) / 100;
            }
            else
            {
                throw new BadRequestException("Loại thanh toán không hợp lệ");
            }
            return totalAmount;
        }


        private async Task SendNotificationToTeller(string contractId, string type, Guid tellerId)
        {
            var message = new CreateNotificationModel
            {
                Title = $"Thanh toán {type} hợp đồng {contractId}",
                Body = $"Khách hàng đã thanh toán thành công tiền {type} cho hợp đồng. Vui lòng kiểm tra và xử lý hợp đồng này.",
                Data = new NotificationDataViewModel
                {
                    CreateAt = DateTime.Now,
                    Type = NotificationType.SurveyReport,
                    Link = contractId
                }
            };

           
            await _notificationService.SendNotification(new List<Guid> { tellerId }, message);
        }
    }
}
