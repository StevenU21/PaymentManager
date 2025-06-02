using PaymentManager.Models;

namespace PaymentManager.Services
{
    public class PaymentMethodService : BaseService<PaymentMethod>, IPaymentMethodService
    {
        public PaymentMethodService(Data.AppDbContext context) : base(context) { }
    }
}
