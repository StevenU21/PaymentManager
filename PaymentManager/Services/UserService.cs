using Microsoft.EntityFrameworkCore;
using PaymentManager.Models;

namespace PaymentManager.Services
{
    public class UserService : BaseService<User>, IUserService
    {
        public UserService(Data.AppDbContext context) : base(context) { }

        public override async Task<List<User>> GetAllAsync()
            => await _context.Users.ToListAsync();

        public override async Task<User?> GetByIdAsync(int id)
            => await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
}