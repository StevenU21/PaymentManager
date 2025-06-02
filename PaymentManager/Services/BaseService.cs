using Microsoft.EntityFrameworkCore;

namespace PaymentManager.Services
{
    public class BaseService<T> where T : class
    {
        protected readonly Data.AppDbContext _context;

        public BaseService(Data.AppDbContext context)
        {
            _context = context;
        }

        public virtual async Task<List<T>> GetAllAsync()
            => await _context.Set<T>().ToListAsync();

        public virtual async Task<T?> GetByIdAsync(int id)
            => await _context.Set<T>().FindAsync(id);

        public virtual async Task AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}