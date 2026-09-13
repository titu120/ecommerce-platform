using Ecommerce.Application.DTOs.Dashboard;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var allOrders = (await _unitOfWork.Orders.GetAllOrdersWithItemsAsync()).ToList();

            // Cancelled Order বাদ দিয়ে হিসাব করা
            var validOrders = allOrders.Where(o => o.Status != OrderStatus.Cancelled).ToList();

            var totalRevenue = validOrders.Sum(o => o.TotalAmount);
            var totalOrders = validOrders.Count;

            var topProducts = validOrders
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    TotalQuantitySold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.UnitPrice * oi.Quantity)
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(5)
                .ToList();

            return new DashboardDto
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TopProducts = topProducts
            };
        }
    }
}