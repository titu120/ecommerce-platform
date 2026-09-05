using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public int Quantity { get; set; }

        // Foreign Keys
        public int CartId { get; set; }
        public int ProductId { get; set; }

        // Navigation Properties
        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}