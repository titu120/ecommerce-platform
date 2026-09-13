using Ecommerce.Application.DTOs.Order;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using FluentValidation;

namespace Ecommerce.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateOrderDto> _createValidator;

        public OrderService(IUnitOfWork unitOfWork, IValidator<CreateOrderDto> createValidator)
        {
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
        }

        public async Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new Ecommerce.Application.Exceptions.ValidationException(validationResult.Errors);
            }

            var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                throw new Ecommerce.Application.Exceptions.ValidationException(
                    new List<FluentValidation.Results.ValidationFailure>
                    {
                        new("Cart", "Your cart is empty.")
                    });
            }

            // Stock আগে থেকেই যথেষ্ট আছে কিনা চেক করা (Transaction শুরুর আগে)
            foreach (var item in cart.CartItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new Ecommerce.Application.Exceptions.NotFoundException("Product", item.ProductId);
                }
                if (product.StockQuantity < item.Quantity)
                {
                    throw new Ecommerce.Application.Exceptions.ValidationException(
                        new List<FluentValidation.Results.ValidationFailure>
                        {
                            new("Stock", $"Insufficient stock for '{product.Name}'. Only {product.StockQuantity} available.")
                        });
                }
            }

            // এখান থেকে Transaction শুরু — Order তৈরি + Stock কমানো + Cart Clear, সব একসাথে অথবা কিছুই না
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    UserId = userId,
                    ShippingAddress = dto.ShippingAddress,
                    Status = OrderStatus.Pending,
                    TotalAmount = 0
                };

                decimal totalAmount = 0;
                var orderItems = new List<OrderItem>();

                foreach (var cartItem in cart.CartItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);

                    var orderItem = new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = product!.Price
                    };
                    orderItems.Add(orderItem);
                    totalAmount += product.Price * cartItem.Quantity;

                    // Stock কমানো
                    product.StockQuantity -= cartItem.Quantity;
                    _unitOfWork.Products.Update(product);
                }

                order.TotalAmount = totalAmount;
                order.OrderItems = orderItems;

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.SaveChangesAsync(); // Order + OrderItems + Product Stock Update — একসাথে Save

                // Checkout শেষে Cart Clear করা
                foreach (var item in cart.CartItems.ToList())
                {
                    _unitOfWork.CartItems.Delete(item);
                }
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync(); // সব ঠিক থাকলে, স্থায়ীভাবে Save

                var createdOrder = await _unitOfWork.Orders.GetOrderWithItemsAsync(order.Id);
                return MapOrderToDto(createdOrder!);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(); // কোথাও Error হলে, সব বাতিল
                throw;
            }
        }

        public async Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userId)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByUserAsync(userId);
            return orders.Select(MapOrderToDto);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int orderId, int userId, bool isAdmin)
        {
            var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);
            if (order == null)
            {
                throw new Ecommerce.Application.Exceptions.NotFoundException("Order", orderId);
            }

            // Admin না হলে, শুধু নিজের Order দেখতে পারবে
            if (!isAdmin && order.UserId != userId)
            {
                throw new Ecommerce.Application.Exceptions.NotFoundException("Order", orderId);
            }

            return MapOrderToDto(order);
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
        {
            var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);
            if (order == null)
            {
                throw new Ecommerce.Application.Exceptions.NotFoundException("Order", orderId);
            }

            order.Status = dto.Status;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return MapOrderToDto(order);
        }

        public async Task CancelOrderAsync(int orderId, int userId)
        {
            var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);
            if (order == null)
            {
                throw new Ecommerce.Application.Exceptions.NotFoundException("Order", orderId);
            }

            if (order.UserId != userId)
            {
                throw new Ecommerce.Application.Exceptions.NotFoundException("Order", orderId);
            }

            if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
            {
                throw new Ecommerce.Application.Exceptions.ValidationException(
                    new List<FluentValidation.Results.ValidationFailure>
                    {
                        new("Status", $"Cannot cancel an order that is already {order.Status}.")
                    });
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Cancel হলে Stock ফেরত দেওয়া
                foreach (var item in order.OrderItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity;
                        _unitOfWork.Products.Update(product);
                    }
                }

                order.Status = OrderStatus.Cancelled;
                _unitOfWork.Orders.Update(order);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(int pageNumber, int pageSize)
        {
            var allOrders = await _unitOfWork.Orders.GetAllOrdersWithItemsAsync();

            var paged = allOrders
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return paged.Select(MapOrderToDto);
        }

        private static OrderDto MapOrderToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                ShippingAddress = order.ShippingAddress,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? string.Empty,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    Subtotal = oi.UnitPrice * oi.Quantity
                }).ToList()
            };
        }
    }
}