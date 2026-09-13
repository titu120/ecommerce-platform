using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities
{
    public class Review : BaseEntity
    {
        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }

        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; } // কোন Order থেকে Review দিচ্ছে, যাচাইয়ের জন্য

        public Product Product { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}