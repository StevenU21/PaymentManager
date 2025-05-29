using Microsoft.EntityFrameworkCore;

namespace PaymentManager.Services
{
    public class UserService : IUserService
    {
        private readonly Data.AppDbContext _context;

        public UserService(Data.AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Models.User>> GetUsersAsync()
            => await _context.Users.Include(u => u.PaymentStatus).ToListAsync();

        public async Task<Models.User?> GetUserByIdAsync(int id)
            => await _context.Users.Include(u => u.PaymentStatus).FirstOrDefaultAsync(u => u.Id == id);

        public async Task AddUserAsync(Models.User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(Models.User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }

}
