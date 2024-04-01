using ISHE_Data.Entities;
using ISHE_Data.Repositories.Interfaces;

namespace ISHE_Data.Repositories.Implementations
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(SMART_HOME_DBContext context) : base(context)
        {
        }
    }
}
