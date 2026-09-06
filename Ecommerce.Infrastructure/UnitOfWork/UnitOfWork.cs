using Ecommerce.Application.Interfaces;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Infrastructure.Repositories;

namespace Ecommerce.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        private IProductRepository? _products;
        private ICategoryRepository? _categories;
        private IOrderRepository? _orders;
        private ICartRepository? _carts;
        private IUserRepository? _users;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products =>
            _products ??= new ProductRepository(_context);

        public ICategoryRepository Categories =>
            _categories ??= new CategoryRepository(_context);

        public IOrderRepository Orders =>
            _orders ??= new OrderRepository(_context);

        public ICartRepository Carts =>
            _carts ??= new CartRepository(_context);

        public IUserRepository Users =>
            _users ??= new UserRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}