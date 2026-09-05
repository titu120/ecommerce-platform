using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }

    // Foreign Key
    public int CategoryId { get; set; }

    // Navigation Property — একটা Product একটাই Category এর হবে
    public Category Category { get; set; } = null!;
}