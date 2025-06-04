using Microsoft.EntityFrameworkCore;
using PaymentManager.Models;

namespace PaymentManager.Services
{
    public class PaymentService : BaseService<Payment>, IPaymentService
    {
        public PaymentService(Data.AppDbContext context) : base(context) { }

        public override async Task<List<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.UserPaymentPlan)
                    .ThenInclude(upp => upp.User)
                .Include(p => p.UserPaymentPlan)
                    .ThenInclude(upp => upp.PaymentPlan)
                .Include(p => p.PaymentMethod)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserPaymentPlan>> GetAllUserPaymentPlansAsync()
        {
            return await _context.UserPaymentPlans
                .Include(upp => upp.User)
                .Include(upp => upp.PaymentPlan)
                .ToListAsync();
        }
    }
}