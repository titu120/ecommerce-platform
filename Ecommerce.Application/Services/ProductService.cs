using Ecommerce.Application.DTOs.Product;
using Ecommerce.Application.Interfaces;

namespace Ecommerce.Application.Services
{
    public class ProductService : IProductService
    {
        public Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ProductDto> GetProductByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductDto>> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductDto>> SearchProductsAsync(string keyword)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold)
        {
            throw new NotImplementedException();
        }
    }
}