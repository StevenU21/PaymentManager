using PaymentManager.Models;

namespace PaymentManager.Services
{
    public class PaymentTypeService : BaseService<PaymentType>, IPaymentTypeService
    {
        public PaymentTypeService(Data.AppDbContext context) : base(context) { }
    }
}
