namespace Ecommerce.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    IOrderRepository Orders { get; }
    ICartRepository Carts { get; }
    IUserRepository Users { get; }

    Task<int> SaveChangesAsync();
}