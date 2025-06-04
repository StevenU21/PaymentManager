using PaymentManager.Models;
using Microsoft.EntityFrameworkCore;

namespace PaymentManager.Services
{
    public class UserPaymentPlanService : BaseService<UserPaymentPlan>, IUserPaymentPlanService
    {
        public UserPaymentPlanService(Data.AppDbContext context) : base(context) { }

        public override async Task<List<UserPaymentPlan>> GetAllAsync()
        {
            var upps = await _context.UserPaymentPlans
                .AsNoTracking()
                .Include(upp => upp.User)
                .Include(upp => upp.PaymentPlan)
                .ToListAsync();
        
            foreach (var plan in upps.Select(u => u.PaymentPlan).Distinct())
            {
                if (plan != null)
                {
                    plan.UserPaymentPlans = await _context.UserPaymentPlans
                        .AsNoTracking()
                        .Where(x => x.PaymentPlanId == plan.Id)
                        .ToListAsync();
                }
            }
        
            return upps;
        }
    }
}