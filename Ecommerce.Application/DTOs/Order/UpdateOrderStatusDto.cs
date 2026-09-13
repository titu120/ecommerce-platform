using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.DTOs.Order
{
    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
    }
}