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
                .Include(p => p.User)
                .Include(p => p.PaymentMethod)
                .Include(p => p.PaymentPlan)
                .ToListAsync();
        }
    }
}