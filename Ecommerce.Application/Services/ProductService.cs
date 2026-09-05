using Ecommerce.Application.DTOs.Product;
using Ecommerce.Application.Exceptions;
using Ecommerce.Application.Interfaces;
using FluentValidation;

namespace Ecommerce.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateProductDto> _createValidator;
        private readonly IValidator<UpdateProductDto> _updateValidator;

        public ProductService(
            IUnitOfWork unitOfWork,
            IValidator<CreateProductDto> createValidator,
            IValidator<UpdateProductDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            throw new NotImplementedException(); // আসল logic পরের ধাপে (E4) আসবে
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            throw new NotImplementedException(); // আসল logic পরের ধাপে (E4) আসবে
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