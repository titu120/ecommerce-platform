using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetReviewsByProductAsync(int productId)
        {
            return await _dbSet
                .Where(r => r.ProductId == productId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasUserReviewedOrderAsync(int userId, int orderId, int productId)
        {
            return await _dbSet.AnyAsync(r =>
                r.UserId == userId && r.OrderId == orderId && r.ProductId == productId);
        }
    }
}