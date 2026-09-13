using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities
{
    public class Wishlist : BaseEntity
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }

        public User User { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}