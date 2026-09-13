using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByProductAsync(int productId);
        Task<bool> HasUserReviewedOrderAsync(int userId, int orderId, int productId);
    }
}