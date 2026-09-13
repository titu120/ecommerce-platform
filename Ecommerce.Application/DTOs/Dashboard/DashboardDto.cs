namespace Ecommerce.Application.DTOs.Dashboard
{
    public class DashboardDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public List<TopProductDto> TopProducts { get; set; } = new();
    }

    public class TopProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}