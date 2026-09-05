using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // Order করার সময়ের দাম (পরে Product এর দাম বদলালেও Order এর দাম ঠিক থাকবে)

        // Foreign Keys
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        // Navigation Properties
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}