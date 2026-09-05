using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation Property — এক Category তে অনেক Product থাকতে পারে
    public ICollection<Product> Products { get; set; } = new List<Product>();
}