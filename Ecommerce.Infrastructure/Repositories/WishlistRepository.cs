using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories
{
    public class WishlistRepository : GenericRepository<Wishlist>, IWishlistRepository
    {
        public WishlistRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Wishlist>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .ToListAsync();
        }

        public async Task<Wishlist?> FindAsync(int userId, int productId)
        {
            return await _dbSet.FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
        }
    }
}