using Ecommerce.Application.DTOs.Order;

namespace Ecommerce.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto dto);
        Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userId);
        Task<OrderDto> GetOrderByIdAsync(int orderId, int userId, bool isAdmin);
        Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto);
        Task CancelOrderAsync(int orderId, int userId);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync(int pageNumber, int pageSize);
    }
}