using Microsoft.EntityFrameworkCore;
using PaymentManager.Models;

namespace PaymentManager.Services
{
    public class PaymentPlanService : BaseService<PaymentPlan>, IPaymentPlanService
    {
        public PaymentPlanService(Data.AppDbContext context) : base(context) { }

        public override async Task<List<PaymentPlan>> GetAllAsync()
        {
            return await _context.PaymentPlans
                .Include(pp => pp.User)
                .Include(pp => pp.PaymentType)
                .ToListAsync();
        }
    }
}