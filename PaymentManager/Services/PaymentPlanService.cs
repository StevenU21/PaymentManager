using Microsoft.EntityFrameworkCore;
using PaymentManager.Models;

namespace PaymentManager.Services
{
    public class PaymentPlanService : BaseService<PaymentPlan>, IPaymentPlanService
    {
        public PaymentPlanService(Data.AppDbContext context) : base(context) { }
    }
}