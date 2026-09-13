using Ecommerce.Application.Interfaces;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ecommerce.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        private IProductRepository? _products;
        private ICategoryRepository? _categories;
        private IOrderRepository? _orders;
        private ICartRepository? _carts;
        private ICartItemRepository? _cartItems;
        private IUserRepository? _users;
        private IReviewRepository? _reviews;
        private IWishlistRepository? _wishlists;

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

        public ICartItemRepository CartItems =>
            _cartItems ??= new CartItemRepository(_context);

        public IUserRepository Users =>
            _users ??= new UserRepository(_context);

        public IReviewRepository Reviews =>
            _reviews ??= new ReviewRepository(_context);

        public IWishlistRepository Wishlists =>
            _wishlists ??= new WishlistRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}